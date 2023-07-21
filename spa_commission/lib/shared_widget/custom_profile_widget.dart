import 'package:flutter/material.dart';
import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:uuid/uuid.dart';
import '../base_client/base_client.dart';
import '../app_theme/app_theme.dart';
import '../models/mobile_user_info.dart';
import '../models/commistion_tier.dart';

class CustomProfileWidget extends StatefulWidget {
  final VoidCallback? onFunctionCall;

  const CustomProfileWidget({
    Key? key,
    this.onFunctionCall,
  }) : super(key: key);

  @override
  CustomProfileWidgetState createState() => CustomProfileWidgetState();
}

class CustomProfileWidgetState extends State<CustomProfileWidget> {
  var formatter = NumberFormat('#,##0.00');
  bool _preload = false;
  String _token = '';
  String _profilePath = '';
  MobileUserInfo _userInfo = MobileUserInfo();
  List<CommissionTierInfo> _comTierInfo = [];
  String? _userName;
  String? _tierName;
  String? _tierColor;
  double _totalBaht = 0;
  double _maxBaht = 0;
  double _totalPercentage = 0;

  @override
  void initState() {
    super.initState();
    loadValue();
  }

  Future<void> loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      setState(() {
        _profilePath = '';
      });
      var res =
          await BaseClient().post('User/GetMoblieUserInfo', {'data': _token});

      if (res != null) {
        setState(() {
          _userInfo = MobileUserInfo.fromJson(res);
          _userName = _userInfo.FirstName?.isEmpty == false &&
                  _userInfo.LastName?.isEmpty == false
              ? '${_userInfo.FirstName} ${_userInfo.LastName}'
              : '';
          _tierName = _userInfo.TierName;
          _tierColor = _userInfo.TierColor;
          _totalBaht = _userInfo.TotalBaht;
          _maxBaht = _userInfo.MaxBaht;
          _totalPercentage = (_totalBaht / _maxBaht);
          _profilePath =
              '${BaseClient().getBaseUrl}File/ProfileImage?token=${_token}&c=${generateUniqueId()}';
        });
      }
    }

    Future.delayed(Duration(milliseconds: 100), () {
      setState(() {
        _preload = false;
      });
    });
  }

  Widget buildLoading() => const CircularProgressIndicator(
        color: CustomTheme.primaryColor,
      );

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 10),
                decoration: BoxDecoration(
                  color: CustomTheme.fillColor,
                ),
                child: _preload
                    ? Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: <Widget>[
                            const SizedBox(height: 70),
                            buildLoading()
                          ])
                    : Row(
                        children: <Widget>[
                          const SizedBox(height: 70),
                          CircleAvatar(
                            radius: 30,
                            backgroundColor: CustomTheme.secondaryColor,
                            backgroundImage: _profilePath != ''
                                ? NetworkImage(_profilePath)
                                : null,
                          ),
                          const SizedBox(width: 20),
                          Expanded(
                            child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: <Widget>[
                                  Text(
                                    _userName ?? '',
                                    style: const TextStyle(
                                        fontSize: 18,
                                        color: CustomTheme.primaryColor),
                                    overflow: TextOverflow.fade,
                                    maxLines: 1,
                                    softWrap: false,
                                  ),
                                  const SizedBox(height: 5),
                                  Row(
                                    children: [
                                      const Text(
                                        'Tier : ',
                                        style: TextStyle(
                                            fontSize: 14,
                                            color: CustomTheme.darkGreyColor),
                                      ),
                                      Container(
                                        padding:
                                            EdgeInsets.fromLTRB(10, 0, 10, 0),
                                        alignment: Alignment.center,
                                        color: Color(int.parse(
                                            _tierColor ?? '000000',
                                            radix: 16)),
                                        child: Text(
                                          _tierName ?? '',
                                          style: const TextStyle(
                                              fontSize: 14,
                                              color: CustomTheme.fillColor),
                                        ),
                                      )
                                    ],
                                  ),
                                  const SizedBox(height: 5),
                                  Container(
                                      decoration: BoxDecoration(
                                        border: Border.all(color: Colors.grey),
                                      ),
                                      width: MediaQuery.of(context).size.width *
                                          0.55,
                                      child: Stack(
                                        children: [
                                          LinearProgressIndicator(
                                            minHeight: 12,
                                            value: _totalPercentage,
                                            backgroundColor:
                                                CustomTheme.fillColor,
                                            valueColor:
                                                AlwaysStoppedAnimation<Color>(
                                                    CustomTheme.primaryColor),
                                          ),
                                          Positioned(
                                            left: 0,
                                            top: 0,
                                            bottom: 0,
                                            right: 0,
                                            child: Align(
                                                alignment: Alignment.center,
                                                child: Text(
                                                  '${formatter.format(_totalBaht)} / ${formatter.format(_maxBaht)}',
                                                  style: TextStyle(fontSize: 8),
                                                )),
                                          ),
                                        ],
                                      ))
                                ]),
                          )
                        ],
                      ),
              ),
            ],
          ),
        )
      ],
    );
  }

  String generateUniqueId() {
    var uuid = Uuid();
    return uuid.v4();
  }
}
