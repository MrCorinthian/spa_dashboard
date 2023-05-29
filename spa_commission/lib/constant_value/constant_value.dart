import 'package:intl/intl.dart';
import '../../base_client/base_client.dart';
import '../models/dropdown_option.dart';
import 'dart:convert';
import 'dart:io';
import 'dart:async';

final List<DropdownOption> MONTHS = List.generate(
    DateFormat.MMMM().dateSymbols.MONTHS.length,
    (i) => DropdownOption(
        value: '${i + 1}',
        label: DateFormat.MMMM().dateSymbols.MONTHS.elementAt(i)));

// final List<DropdownOption> BANK_ACCOUNT = [
//   DropdownOption(value: '1', label: 'KBANK'),
//   DropdownOption(value: '2', label: 'SCB'),
//   DropdownOption(value: '3', label: 'TMB')
// ];

// final List<DropdownOption> PROVINCE = [
//   DropdownOption(value: '1', label: 'กรุงเทพ'),
//   DropdownOption(value: '2', label: 'เชียงใหม่'),
//   DropdownOption(value: '3', label: 'ชลบุรี')
// ];

Future<List<String>> OCCUPATION() async {
  List<String> result = (jsonDecode(await BaseClient()
          .get('Data/GetMobileOptionSetting?code=OCCUPATION')) as List<dynamic>)
      .cast<String>();
  return result;
}
