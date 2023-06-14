import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_text_field.dart';
import '../../../shared_widget/custom_alert_dialog.dart';
import '../../models/responsed_data.dart';
import '../../models/otp_data.dart';

class OtpForgotPasswordPage extends StatefulWidget {
  const OtpForgotPasswordPage({
    Key? key,
  }) : super(key: key);

  @override
  State<OtpForgotPasswordPage> createState() => _OtpForgotPasswordState();
}

class _OtpForgotPasswordState extends State<OtpForgotPasswordPage> {
  String _token = '';
  String _ref = '';
  String _otp = '';
  final TextEditingController _phoneNumberController = TextEditingController();
  final TextEditingController _otpController = TextEditingController();
  final TextEditingController _newPasswordController = TextEditingController();
  final TextEditingController _confirmNewPasswordController =
      TextEditingController();
  bool _isPasswordMatched = true;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      // setState(() {});
    }
  }

  requestOtp() async {
    var res = await BaseClient().post('User/RequestOtpForgotPassword', {
      "PhoneNumber": _phoneNumberController.text,
    });
    if (res != null) {
      OtpData otp = OtpData.fromJson(res);
      setState(() {
        _ref = otp.Ref;
        FocusScope.of(context).unfocus();
      });
    } else {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return buildPopup("Telephone no. does not exist");
        },
      );
    }
  }

  verifyOtp() async {
    var res = await BaseClient().post('User/VerifyOtpForgotPassword', {
      "PhoneNumber": _phoneNumberController.text,
      "Ref": _ref,
      "Otp": _otpController.text,
    });
    if (res != null) {
      OtpData otp = OtpData.fromJson(res);
      setState(() {
        _otp = otp.Otp;
        FocusScope.of(context).unfocus();
      });
    } else {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return buildPopup("OTP is invalid");
        },
      );
    }
  }

  forgotPassword() async {
    var validate = true;
    if (_newPasswordController.text.isEmpty ||
        _confirmNewPasswordController.text.isEmpty ||
        _newPasswordController.text != _confirmNewPasswordController.text) {
      validate = false;
    }

    if (validate) {
      var response = await BaseClient().post('User/ForgotPassword', {
        "PhoneNumber": _phoneNumberController.text,
        "Ref": _ref,
        "Otp": _otp,
        "NewPassword": _newPasswordController.text,
        "ConfirmNewPassword": _confirmNewPasswordController.text
      });
      if (response != null) {
        ResponsedData res = ResponsedData.fromJson(response);
        if (res.success) {
          showDialog(
            context: context,
            builder: (BuildContext context) {
              return buildPopupSuccess();
            },
          );
        }
      } else {
        validate = false;
      }
    }
    if (!validate) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return buildPopup("The confirm new password does not match");
        },
      );
    }
  }

  Widget buildPopupSuccess() {
    return AlertDialog(
      backgroundColor: CustomTheme.darkGreyColor,
      content: Column(mainAxisSize: MainAxisSize.min, children: const [
        SizedBox(
          height: 20,
        ),
        Image(
          image: AssetImage('assets/images/circle-check.png'),
          width: 100,
        ),
        SizedBox(
          height: 20,
        ),
        Text(
          'Completed',
          style: TextStyle(color: CustomTheme.fillColor),
        ),
      ]),
      actions: [
        Center(
          child: TextButton(
            child: Text(
              'Ok',
              style: TextStyle(color: CustomTheme.fillColor, fontSize: 16),
            ),
            style: TextButton.styleFrom(
              backgroundColor: CustomTheme.primaryColor,
            ),
            onPressed: () {
              Navigator.of(context).pop();
              Navigator.of(context).pop();
            },
          ),
        ),
      ],
    );
  }

  Widget buildPopup(String message) {
    return CustomAlertDialog(
      title: 'Message',
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(message.isNotEmpty ? message : 'Please check the information.',
              style: TextStyle(color: CustomTheme.fillColor))
        ],
      ),
    );
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
              children: <Widget>[
                Row(
                  children: const <Widget>[
                    Text(
                      'Forgot password',
                      style: CustomTheme.headerPageName,
                    )
                  ],
                ),
                const SizedBox(
                  height: 20,
                ),
                _ref.isEmpty && _otp.isEmpty
                    ? Column(
                        children: [
                          CustomTextField(
                            text: 'Telephone no.',
                            requiredField: true,
                            keyboardType: 'number',
                            controller: _phoneNumberController,
                          ),
                          const SizedBox(height: 60),
                          ElevatedButton(
                            style: CustomTheme.buttonStyle_primaryColor,
                            onPressed: () => requestOtp(),
                            child: const Text('Confirm',
                                style: CustomTheme.buttonTextStyle_fillColor),
                          )
                        ],
                      )
                    : (_ref.isNotEmpty && _otp.isEmpty
                        ? Column(
                            children: [
                              CustomTextField(
                                text: 'OTP from SMS',
                                requiredField: true,
                                keyboardType: 'number',
                                controller: _otpController,
                              ),
                              const SizedBox(height: 60),
                              ElevatedButton(
                                style: CustomTheme.buttonStyle_primaryColor,
                                onPressed: () => verifyOtp(),
                                child: const Text('Confirm',
                                    style:
                                        CustomTheme.buttonTextStyle_fillColor),
                              ),
                            ],
                          )
                        : Column(
                            children: [
                              CustomTextField(
                                text: 'New password',
                                requiredField: true,
                                obscureText: true,
                                controller: _newPasswordController,
                              ),
                              CustomTextField(
                                text: 'Confirm new password',
                                requiredField: true,
                                obscureText: true,
                                controller: _confirmNewPasswordController,
                                inputDecorationError: _isPasswordMatched,
                                onChanged: (value) {
                                  setState(() {
                                    _isPasswordMatched =
                                        _newPasswordController.text == value;
                                  });
                                },
                              ),
                              const SizedBox(height: 60),
                              ElevatedButton(
                                style: CustomTheme.buttonStyle_primaryColor,
                                onPressed: () => forgotPassword(),
                                child: const Text('Confirm',
                                    style:
                                        CustomTheme.buttonTextStyle_fillColor),
                              ),
                            ],
                          )),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
