import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_profile_widget.dart';
import '../../../shared_widget/custom_container.dart';
import '../../../shared_widget/custom_page_route_builder.dart';
import '../../../models/user_report.dart';
import './qr_scan.dart';

class HomePage extends StatefulWidget {
  const HomePage({
    Key? key,
  }) : super(key: key);

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  GlobalKey<CustomProfileWidgetState> childWidgetKey =
      GlobalKey<CustomProfileWidgetState>();

  var formatter = NumberFormat('#,##0.00');
  DateTime now = DateTime.now();
  String _token = '';
  List<UserReport> _userReport = [];

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      _getReport();
    }
  }

  Future<void> _getReport() async {
    var res = await BaseClient().post('Report/GetReport', {
      'token': _token,
      'month': DateFormat('MMMM').format(now),
      'year': DateFormat('yyyy').format(now)
    });
    if (res != null) {
      final jsonData = json.decode(res) as List;
      setState(() {
        _userReport =
            jsonData.map((json) => UserReport.fromJson(json)).toList();
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        CustomProfileWidget(key: childWidgetKey),
        const SizedBox(
          height: 10,
        ),
        CustomContainer(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              const Text(
                'Period',
                style: CustomTheme.textStyle_lightText,
              ),
              const SizedBox(
                height: 4,
              ),
              Text(
                DateFormat('MMMM yyyy').format(now),
                style: CustomTheme.headerPageName,
              ),
              const SizedBox(
                height: 20,
              ),
              const Text(
                'Current period balance',
                style: CustomTheme.textStyle_lightText,
              ),
              const SizedBox(
                height: 4,
              ),
              Text(
                _userReport.isNotEmpty
                    ? '${formatter.format(_userReport.map((item) => item.TotalBath).reduce((a, b) => a + b))} THB'
                    : '0.00 THB',
                style: CustomTheme.headerPageName,
              ),
            ],
          ),
        ),
        const SizedBox(
          height: 54,
        ),
        GestureDetector(
            onTap: () async {
              final result = await Navigator.push(
                context,
                CustomPageRouteBuilder.bottomToTop(const QrScanPage()),
              );

              if (result == true) {
                _getReport();
                childWidgetKey.currentState?.loadValue();
              }
            },
            child: Column(
              children: <Widget>[
                Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(20),
                    color: CustomTheme.primaryColor,
                  ),
                  child: const Image(
                    image: AssetImage('assets/images/qr-code.png'),
                    width: 200,
                  ),
                ),
                const SizedBox(
                  height: 10,
                ),
                const Text(
                  'Scan QR code',
                  style: CustomTheme.textStyle_lightText,
                ),
              ],
            )),
      ],
    );
  }
}
