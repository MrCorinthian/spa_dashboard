import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../base_client/base_client.dart';
import '../../providers/AuthProvider/auth_provider.dart';
import '../../app_theme/app_theme.dart';
import '../register/register.dart';
import 'otp_forgot_password.dart';
import '../../shared_widget/custom_text_field.dart';
import '../../shared_widget/custom_page_route_builder.dart';
import '../../shared_widget/custom_loading.dart';
import '../../shared_widget/custom_alert_dialog.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final TextEditingController _phoneController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    var preLoading = true;
    final prefs = await SharedPreferences.getInstance();
    final _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      var res =
          await BaseClient().post('User/GetMoblieUserInfo', {'data': _token});
      if (res != null) {
        Provider.of<AuthProvider>(context, listen: false)
            .login(jsonEncode({'Success': true, 'Data': _token}));
      } else {
        preLoading = false;
      }
    } else {
      preLoading = false;
    }
    if (!preLoading) {
      setState(() {
        _loading = false;
      });
    }
  }

  void login() async {
    setState(() {
      _loading = true;
    });
    var res = await BaseClient().post('User/Login', {
      'phone': _phoneController.text.trim(),
      'password': _passwordController.text.trim()
    });
    if (res != null) {
      setState(() {
        _loading = false;
        Provider.of<AuthProvider>(context, listen: false).login(res);
      });
    } else {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return CustomAlertDialog(
            title: 'Message',
            child: Text('Please check the information.',
                style: TextStyle(color: CustomTheme.fillColor)),
          );
        },
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return CustomLoading(
      isLoading: _loading,
      child: buildLogin(),
    );
  }

  Widget buildLogin() => Scaffold(
        body: Container(
            padding: CustomTheme.paddingPage,
            child: Column(
              children: <Widget>[
                const Image(
                  image: AssetImage('assets/images/urban-logo.png'),
                ),
                CustomTextField(
                  text: 'Telephone no.',
                  controller: _phoneController,
                  keyboardType: 'number',
                ),
                CustomTextField(
                    text: 'Password',
                    obscureText: true,
                    controller: _passwordController),
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
                      style:
                          TextStyle(fontSize: 16, color: CustomTheme.fillColor),
                    ),
                  ),
                )),
                const SizedBox(height: 30),
                ElevatedButton(
                  style: CustomTheme.buttonStyle_primaryColor,
                  onPressed: login,
                  child: const Text('Sign in',
                      style: CustomTheme.buttonTextStyle_fillColor),
                ),
              ],
            )),
        bottomNavigationBar: Container(
          padding: CustomTheme.paddingPage,
          child: ElevatedButton(
            style: CustomTheme.buttonStyle_secondaryColor,
            onPressed: () {
              Navigator.push(
                context,
                PageRouteBuilder(
                  transitionDuration: const Duration(milliseconds: 500),
                  transitionsBuilder: (context, animation, _, child) {
                    return SlideTransition(
                      position: Tween<Offset>(
                        begin: const Offset(-1, 0),
                        end: Offset.zero,
                      ).animate(animation),
                      child: child,
                    );
                  },
                  pageBuilder: (_, __, ___) => RegisterPage(),
                ),
              );
            },
            child: const Text('Register',
                style: CustomTheme.buttonTextStyle_fillColor),
          ),
        ),
      );
}
