import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../providers/AuthProvider/auth_provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';
import '../../login/otp_forgot_password.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_text_field.dart';
import '../../../shared_widget/custom_alert_dialog.dart';
import '../../../shared_widget/custom_page_route_builder.dart';
import '../../../shared_widget/custom_loading.dart';
import '../../../models/responsed_data.dart';

class ChangePasswordPage extends StatefulWidget {
  const ChangePasswordPage({
    Key? key,
  }) : super(key: key);

  @override
  State<ChangePasswordPage> createState() => _ChangePasswordState();
}

class _ChangePasswordState extends State<ChangePasswordPage> {
  String _token = '';
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _newPasswordController = TextEditingController();
  final TextEditingController _confirmNewPasswordController =
      TextEditingController();
  bool _isPasswordMatched = true;
  bool _loading = false;

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

  ChangePassword() async {
    if (!_loading) {
      setState(() {
        _loading = true;
      });

      bool verifiedPassword = false;
      var response = await BaseClient().post('User/Password',
          {"Token": _token, "Password": _passwordController.text});
      if (response != null) {
        ResponsedData resPassword = ResponsedData.fromJson(response);
        if (resPassword.success == true) {
          verifiedPassword = true;
        }
      }

      if (verifiedPassword) {
        var validate = true;
        if (_newPasswordController.text.isEmpty ||
            _confirmNewPasswordController.text.isEmpty ||
            _newPasswordController.text != _confirmNewPasswordController.text ||
            _newPasswordController.text.length < 6) {
          validate = false;
        }

        if (validate) {
          var res = await BaseClient().post('User/ChangePassword', {
            "Token": _token,
            "Password": _passwordController.text,
            "NewPassword": _newPasswordController.text,
            "ConfirmNewPassword": _confirmNewPasswordController.text
          });
          if (res != null) {
            Provider.of<AuthProvider>(context, listen: false).login(res);
            Navigator.of(context).pop();
            Navigator.of(context).pop();
          } else {
            validate = false;
          }
        }

        if (!validate) {
          showDialog(
            context: context,
            builder: (BuildContext context) {
              return buildPopup(_newPasswordController.text.length < 6
                  ? "A new password must be more than 6 character"
                  : "The confirm new password does not match");
            },
          );
        }
      } else {
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return buildPopup(
                'Please check your current password and try again.');
          },
        );
      }
    }
    setState(() {
      _loading = false;
    });
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
    return CustomLoading(
      isLoading: _loading,
      child: buildWidget(context),
    );
  }

  Widget buildWidget(BuildContext context) {
    return GestureDetector(
        onTap: () {
          FocusManager.instance.primaryFocus?.unfocus();
        },
        child: Scaffold(
          appBar: const CustomAppBar(),
          body: SingleChildScrollView(
            child: Center(
              child: Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
                child: Column(
                  children: <Widget>[
                    Row(
                      children: const <Widget>[
                        Text(
                          'Change password',
                          style: CustomTheme.headerPageName,
                        )
                      ],
                    ),
                    const SizedBox(
                      height: 20,
                    ),
                    CustomTextField(
                      text: 'Current password',
                      requiredField: true,
                      obscureText: true,
                      controller: _passwordController,
                    ),
                    Container(
                        child: Align(
                      alignment: Alignment.topRight,
                      child: TextButton(
                        style: TextButton.styleFrom(
                          padding: const EdgeInsets.fromLTRB(0, 10, 0, 0),
                        ),
                        onPressed: () => Navigator.push(
                            context,
                            CustomPageRouteBuilder.bottomToTop(
                                OtpForgotPasswordPage())),
                        child: const Text(
                          'Forgot password',
                          style: TextStyle(
                              fontSize: 16, color: CustomTheme.fillColor),
                        ),
                      ),
                    )),
                    const SizedBox(
                      height: 40,
                    ),
                    CustomTextField(
                      text: 'New password',
                      placeholder: 'Must have at least 6 characters',
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
                      onPressed: () => ChangePassword(),
                      child: const Text('Confirm',
                          style: CustomTheme.buttonTextStyle_fillColor),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ));
  }
}
