import 'package:flutter/material.dart';
import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:spa_commission/models/user_report.dart';
import '../../../base_client/base_client.dart';
import '../../../constant_value/constant_value.dart' as constent;
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_dropdown_side_by_side.dart';
import '../../../shared_widget/custom_container.dart';
import '../../../models/dropdown_option.dart';

class ReportPage extends StatefulWidget {
  const ReportPage({
    Key? key,
  }) : super(key: key);

  @override
  State<ReportPage> createState() => _ReportPageState();
}

class _ReportPageState extends State<ReportPage> {
  var formatter = NumberFormat('#,##0.00');
  DateTime now = DateTime.now();
  final TextEditingController _monthController = TextEditingController();
  final TextEditingController _yearController = TextEditingController();

  String _token = '';
  List<UserReport> _userReport = [];
  bool paymentStatus = false;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      setState(() {
        _monthController.text = DateFormat('MMMM').format(now);
        _yearController.text = DateFormat('yyyy').format(now);
        _getReport();
      });
    }
  }

  Future<void> _getReport() async {
    var res = await BaseClient().post('Report/GetReport', {
      'token': _token,
      'month': _monthController.text,
      'year': _yearController.text
    });
    setState(() {
      _userReport = [];
    });
    if (res != null) {
      final jsonData = json.decode(res) as List;
      setState(() {
        _userReport =
            jsonData.map((json) => UserReport.fromJson(json)).toList();
        paymentStatus =
            _userReport.where((c) => c.PaymentStatus == "N").length == 0
                ? true
                : false;
      });
    }
  }

  Widget buildLoadingWidget() => const Center(
        child: CircularProgressIndicator(
          color: CustomTheme.primaryColor,
        ),
      );

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Text(
            'Select report period',
            style: CustomTheme.textStyle_lightText,
          ),
          const SizedBox(
            height: 10,
          ),
          LayoutBuilder(builder: (context, constraints) {
            return Row(
              children: <Widget>[
                CustomDropdownSideBySide(
                  maxWidth: (constraints.maxWidth * 0.5) - 10,
                  options: constent.MONTHS,
                  selected: _monthController.text,
                  onChanged: (value) {
                    setState(() {
                      _monthController.text = value ?? '';
                      _getReport();
                    });
                  },
                ),
                const SizedBox(
                  width: 20,
                ),
                CustomDropdownSideBySide(
                  maxWidth: (constraints.maxWidth * 0.5) - 10,
                  options: [DropdownOption(value: '2023', label: '2023')],
                  selected: _yearController.text,
                  onChanged: (value) {
                    setState(() {
                      _yearController.text = value ?? '';
                    });
                  },
                ),
              ],
            );
          }),
          const SizedBox(
            height: 40,
          ),
          LayoutBuilder(builder: (context, constraints) {
            return Row(
              children: <Widget>[
                Container(
                  width: (constraints.maxWidth * 0.5) - 10,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: <Widget>[
                      const Text(
                        'Period balance',
                        style: CustomTheme.textStyle_lightText,
                      ),
                      const SizedBox(
                        height: 5,
                      ),
                      Text(
                        _userReport.isNotEmpty
                            ? '${formatter.format(_userReport.map((item) => item.TotalBath).reduce((a, b) => a + b))}'
                            : '0.00',
                        style: CustomTheme.headerPageName,
                      ),
                    ],
                  ),
                ),
                const SizedBox(
                  width: 20,
                ),
                Container(
                  width: (constraints.maxWidth * 0.5) - 10,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: <Widget>[
                      const Text(
                        'Payment status',
                        style: CustomTheme.textStyle_lightText,
                      ),
                      const SizedBox(
                        height: 5,
                      ),
                      if (paymentStatus)
                        const Text(
                          'Completed',
                          style: CustomTheme.headerPageNameGreen,
                        )
                      else
                        const Text(
                          'None',
                          style: CustomTheme.headerPageName,
                        ),
                    ],
                  ),
                )
              ],
            );
          }),
          const SizedBox(
            height: 20,
          ),
          CustomContainer(
            child: Column(
              children: <Widget>[
                for (int i = 0; i < _userReport.length; i++)
                  LayoutBuilder(builder: (context, constraints) {
                    return Container(
                      padding: EdgeInsets.fromLTRB(0, 0, 0, 10),
                      child: Row(
                        children: <Widget>[
                          Container(
                            width: (constraints.maxWidth * 0.5),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: <Widget>[
                                Text(
                                  '${DateFormat('dd/MM/yyyy HH:mm').format(_userReport[i].Created)}',
                                  style: TextStyle(
                                      fontSize: 16,
                                      color: CustomTheme.fillColor),
                                ),
                              ],
                            ),
                          ),
                          Container(
                            width: (constraints.maxWidth * 0.5),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.end,
                              children: <Widget>[
                                Text(
                                  '${formatter.format(_userReport[i].TotalBath)} Baht',
                                  style: TextStyle(
                                      fontSize: 16,
                                      color: CustomTheme.fillColor),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    );
                  })
              ],
            ),
          )
        ],
      ),
    );
  }
}
