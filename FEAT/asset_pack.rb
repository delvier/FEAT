#!/usr/bin/env ruby

def hexify(value)
  "0x#{sprintf('%02x%02x%02x%02x', *value).upcase}"
end
  
def read_long(file)
  b1 = file.readbyte
  b2 = file.readbyte
  b3 = file.readbyte
  b4 = file.readbyte
  b4 << 24 | b3 << 16 | b2 << 8 | b1
end

def unpack(path)
  file = File.open(path, 'r+b')
  fsize = read_long(file)
  data_size = read_long(file)
  pointer_count_1 = read_long(file)
  pointer_count_2 = read_long(file)
  file.read(16)
  data_region = file.read(data_size).unpack('C*')
  data_bak = data_region.clone
  ptr1_region = file.read(pointer_count_1 * 4).unpack('C*')
  ptr2_region = file.read(pointer_count_2 * 8).unpack('C*')
  end_region = file.read(fsize - 0x20 - data_size).unpack('C*')
  end_ofs = data_size + ptr1_region.size + ptr2_region.size
  idx = 0
  pointer_1 = {}
  until ptr1_region.empty?
    pointer_1[ptr1_region.shift(4).pack('C*').unpack('L<').first] = idx
    idx += 4
  end
  pointer_2 = {}
  until ptr2_region.empty?
    key = ptr2_region.shift(4).pack('C*').unpack('L<').first
    (pointer_2[key] ||= []) << ptr2_region.shift(4).pack('C*').unpack('L<').first + end_ofs
  end
  labels = {}
  idx = end_ofs
  until end_region.empty?
    str = []
    str << end_region.shift until str.last == 0 || end_region.empty?
    sz = str.size
    str.pop
    labels[idx] = str.pack('C*').force_encoding('shift_jis').encode('utf-8')
    idx += sz
  end
  data = {}
  idx = 0
  ptr1s = {}
  until data_region.empty?
    raw = data_region.shift(4)
    ptr = raw.pack('C*').unpack('L<').first
    if pointer_1[idx]
      if ptr < end_ofs
        pid = ptr1s[ptr]
        unless pid
          pid = ptr1s.size
          ptr1s[ptr] = pid
        end
        data[idx] = ["POINTER1: #{pid}", :ptr1]
      else
        data[idx] = [ptr, labels[ptr]]
      end
    end
    if (p2 = pointer_2[idx])
      if data[idx]
        data[idx][1] = :ptr3
        data[idx][2] = labels[ptr]
        data[idx][3] = p2
      else
        data[idx] = [ptr, :ptr2, labels[ptr], p2]
      end
    end
    data[idx] = [ptr] unless data[idx]
    idx += 4
  end
  File.open("#{path}.txt", 'w+') do |f|
    data.each_pair do |k,ptr|
      if p = ptr1s[k]
        f.write("PTR1: #{p}\n")
      end
      if ptr[1]
        if ptr[1] == :ptr1
          f.write("#{ptr[0]}\n")
        elsif ptr[1] == :ptr2
          ptr[3].each do |ptr2|
            f.write("PTR2: #{labels[ptr2]}\n")
          end
          if ptr[2]
            f.write("#{ptr[2]}\n")
          else
            f.write("#{hexify([ptr[0]].pack('L<').unpack('CCCC'))}\n")
          end
        elsif ptr[1] == :ptr3
          ptr[3].each do |ptr3|
            f.write("PTR2: #{labels[ptr3]}\n")
          end
          if ptr[0].is_a?(String)
            f.write("#{ptr[0]}\n")
          end
          if ptr[2]
            f.write("#{ptr[2]}\n")
          elsif !ptr[0].is_a?(String)
            f.write("#{hexify([ptr[0]].pack('L<').unpack('CCCC'))}\n")
          end
        else
          f.write("#{ptr[1]}\n")
        end
      else
        f.write("#{hexify([ptr[0]].pack('L<').unpack('CCCC'))}\n")
      end
    end
  end
end

def pack(path, out)
  data = File.open(path, 'r') { |f| f.readlines }
  data_region = []
  labels = []
  end_region = []
  pointer_1 = []
  pointer1_dr = []
  pointer1_lbl = {}
  ptr1s = {}
  pointer_2 = []
  pointer2_labels = []
  other_labels = []
  data.each_with_index do |line,i|
    line.strip!
    if line[/^PTR1: (.+)/]
      ptr1s[$1] = [data_region.size * 4].pack('L<').unpack('C*')
    elsif line[/^POINTER1: (.+)/]
      data_region << line
    elsif line[/^PTR2: (.+)/]
      pointer_2 << [data_region.size * 4, $1]
      pointer2_labels << $1
    elsif line[/^0x(\w+)$/]
      data_region << [$1.to_i(16)].pack('L<').unpack('CCCC').reverse
    else
      other_labels << line
      data_region << line
    end
  end
  labels.concat(pointer2_labels).concat(other_labels)
  label_idx = {}
  label_idx_raw = {}
  labels.uniq!
  end_ofs = data_region.size * 4 + pointer_2.size * 8
  data_region.each { |d| end_ofs += 4 if d.is_a?(String) }
  size = end_ofs
  labels.each_with_index do |label,i|
    real = label.encode('shift_jis').bytes << 0
    label_idx[label] = [size].pack('L<').unpack('C*')
    label_idx_raw[label] = size
    end_region << real
    size += real.size
  end
  pointer_2.sort_by! { |p| p[0] }
  pointer_2.each do |ptr|
    ptr[0] = [ptr[0]].pack('L<').unpack('C*')
    ptr[1] = [label_idx_raw[ptr[1]] - end_ofs].pack('L<').unpack('C*')
  end
  data_region.each_with_index do |d,i|
    if d.is_a?(String) && d[/^POINTER1: (.+)/]
      data_region[i] = ptr1s[$1]
      puts "NULL: #{$1}" unless ptr1s[$1]
      pointer1_dr << i * 4
    elsif (ptr = label_idx[d])
      data_region[i] = ptr
      (pointer1_lbl[d] ||= []) << i * 4
    end
  end
  pointer1_dr.sort!
  pointer_1.concat(pointer1_dr.collect { |i| [i].pack('L<').unpack('C*') })
  ptr1_lbl_sort = []
  pointer1_lbl.each_value { |v| ptr1_lbl_sort << v.sort }
  ptr1_lbl_sort.sort_by! { |v| v[0] }
  ptr1_lbl_sort.flatten!
  pointer_1.concat(ptr1_lbl_sort.collect { |i| [i].pack('L<').unpack('C*') })
  data_region.flatten!
  pointer_1.flatten!
  pointer_2.flatten!
  end_region.flatten!
  header = []
  size = 0x20 + data_region.size + pointer_1.size + pointer_2.size + end_region.size
  header.concat([size].pack('L<').unpack('CCCC'))
  header.concat([data_region.size].pack('L<').unpack('CCCC'))
  header.concat([pointer_1.size / 4].pack('L<').unpack('CCCC'))
  header.concat([pointer_2.size / 8].pack('L<').unpack('CCCC'))
  header.concat([0] * 16)
  File.open(out, 'w+b') do |f|
    f.write(header.pack('C*'))
    f.write(data_region.pack('C*'))
    f.write(pointer_1.pack('C*'))
    f.write(pointer_2.pack('C*'))
    f.write(end_region.pack('C*'))
  end
end

case ARGV[0]
when '-u'
  unpack(ARGV[1])
when '-p'
  pack(ARGV[1], ARGV[2])
else
  puts 'Fire Emblem Asset Pack v1.3'
  puts
  puts 'USAGE:'
  puts '  unpack: asset_pack.rb -u IN'
  puts '  pack: asset_pack.rb -p IN OUT'
  puts
  puts '  A file will unpack to NAME_data.txt. Edit said file freely, then'
  puts '  repack it. You\'re done.'
  puts
  puts '  This tool should work for any Fire Emblem Fates file with a .bin'
  puts '  extension, and it will properly handle all pointer types.'
end
