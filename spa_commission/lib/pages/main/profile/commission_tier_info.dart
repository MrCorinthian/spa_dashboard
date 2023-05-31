import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_container.dart';
import '../../../models/commistion_tier.dart';

class CommisionTierInfoPage extends StatefulWidget {
  const CommisionTierInfoPage({
    Key? key,
  }) : super(key: key);

  @override
  State<CommisionTierInfoPage> createState() => _CommisionTierInfoState();
}

class _CommisionTierInfoState extends State<CommisionTierInfoPage> {
  var formatter = NumberFormat('#,##0.00');
  var formatterPercentage = NumberFormat('#,###');
  String _token = '';
  List<CommissionTierInfo> _comTierInfo = [];

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      var res =
          await BaseClient().post('Data/GetCommissionTier', {'data': _token});

      final jsonData = json.decode(res) as List;
      setState(() {
        _comTierInfo =
            jsonData.map((json) => CommissionTierInfo.fromJson(json)).toList();
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CustomAppBar(),
      body: SingleChildScrollView(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                Row(
                  children: const <Widget>[
                    Text(
                      'Commission tier',
                      style: CustomTheme.headerPageName,
                    )
                  ],
                ),
                const SizedBox(
                  height: 20,
                ),
                for (int i = 0; i < _comTierInfo.length; i++)
                  buildTierInfo(_comTierInfo[i]),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget buildTierInfo(CommissionTierInfo item) => Container(
      padding: EdgeInsets.fromLTRB(0, 0, 0, 5),
      child: CustomContainer(
          child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Container(
            padding: EdgeInsets.all(5),
            width: 100,
            alignment: Alignment.center,
            color: Color(int.parse(item.TierColor, radix: 16)),
            child: Text(
              item.TierName,
              style: TextStyle(
                color: CustomTheme.fillColor,
              ),
            ),
          ),
          const SizedBox(
            height: 10,
          ),
          if (item.ComBahtTo != null)
            Text(
              '${formatter.format(item.ComBahtFrom)} - ${formatter.format(item.ComBahtTo)} Baht',
              style: CustomTheme.textStyle_lightText,
            )
          else
            Text(
              'More than ${formatter.format(item.ComBahtFrom)} Baht',
              style: CustomTheme.textStyle_lightText,
            ),
          const SizedBox(
            height: 10,
          ),
          Text(
            'Commission ${formatterPercentage.format(item.ComPercentage)}%',
            style: TextStyle(fontSize: 16, color: CustomTheme.secondaryColor),
          ),
        ],
      )));
}
