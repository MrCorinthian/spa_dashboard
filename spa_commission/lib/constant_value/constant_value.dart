import 'package:intl/intl.dart';
import '../models/dropdown_option.dart';

final List<DropdownOption> MONTHS = List.generate(
    DateFormat.MMMM().dateSymbols.MONTHS.length,
    (i) => DropdownOption(
        value: '${i + 1}',
        label: DateFormat.MMMM().dateSymbols.MONTHS.elementAt(i)));

final List<DropdownOption> BANK_ACCOUNT = [
  DropdownOption(value: '1', label: 'KBANK'),
  DropdownOption(value: '2', label: 'SCB'),
  DropdownOption(value: '3', label: 'TMB')
];

final List<DropdownOption> PROVINCE = [
  DropdownOption(value: '1', label: 'กรุงเทพ'),
  DropdownOption(value: '2', label: 'เชียงใหม่'),
  DropdownOption(value: '3', label: 'ชลบุรี')
];

final List<DropdownOption> OCCUPATION = [
  DropdownOption(value: '1', label: 'พนักงานบริษัท'),
  DropdownOption(value: '2', label: 'รับราชกาล'),
  DropdownOption(value: '3', label: 'อื่นๆ')
];
