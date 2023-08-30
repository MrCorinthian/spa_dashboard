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

class DeleteAccountPage extends StatefulWidget {
  const DeleteAccountPage({
    Key? key,
  }) : super(key: key);

  @override
  State<DeleteAccountPage> createState() => _DeleteAccountState();
}

class _DeleteAccountState extends State<DeleteAccountPage> {
  String _token = '';
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _deleteConfirmController =
      TextEditingController();
  bool _isConfirmDeleteMatched = true;
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

  Future<void> logout() => SharedPreferences.getInstance().then((prefs) =>
      BaseClient().post('User/Logout', {'data': _token}).then((res) => prefs
          .remove('spa_login_token')
          .then((value) =>
              Provider.of<AuthProvider>(context, listen: false).logout())));

  DeleteAccount() async {
    setState(() {
      _isConfirmDeleteMatched =
          _deleteConfirmController.text.toLowerCase() == 'delete';
    });
    if (!_loading && _deleteConfirmController.text.toLowerCase() == 'delete') {
      setState(() {
        _loading = true;
      });

      var res = await BaseClient().post('User/DeleteAccount',
          {"Token": _token, "Password": _passwordController.text});
      ResponsedData response = ResponsedData.fromJson(res);
      Navigator.of(context).pop();
      if (response.success) {
        setState(() {
          _loading = false;
        });
        logout();
      } else {
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return buildPopup('');
          },
          barrierDismissible: false,
        );
      }

      Navigator.of(context).pop();

      setState(() {
        _loading = false;
      });
    }
  }

  ConfirmPassword() async {
    FocusScope.of(context).unfocus();
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
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return buildPopupConfirmDelete();
          },
          barrierDismissible: false,
        );
      } else {
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return buildPopup('Please check your password and try again.');
          },
          barrierDismissible: false,
        );
      }
    }
    setState(() {
      _loading = false;
    });
  }

  Widget buildPopupConfirmDelete() {
    return AlertDialog(
      backgroundColor: CustomTheme.darkGreyColor,
      title: Text('Confirm delete',
          style: TextStyle(color: CustomTheme.fillColor)),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Are you about to delete the user account ?',
              style: TextStyle(color: CustomTheme.fillColor)),
          const SizedBox(
            height: 20,
          ),
          CustomTextField(
            text: 'To confirm, type \'delete\'',
            requiredField: true,
            controller: _deleteConfirmController,
            inputDecorationError: _isConfirmDeleteMatched,
          ),
          const SizedBox(height: 30),
          ElevatedButton(
            style: CustomTheme.buttonStyle_primaryColor,
            onPressed: () => DeleteAccount(),
            child: const Text('Confirm',
                style: CustomTheme.buttonTextStyle_fillColor),
          ),
          const SizedBox(height: 20),
          ElevatedButton(
            style: CustomTheme.buttonStyle_primaryColor,
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Back',
                style: CustomTheme.buttonTextStyle_fillColor),
          ),
        ],
      ),
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
                          'Delete account',
                          style: CustomTheme.headerPageName,
                        )
                      ],
                    ),
                    const SizedBox(
                      height: 20,
                    ),
                    CustomTextField(
                      text: 'Confirm password',
                      requiredField: true,
                      obscureText: true,
                      controller: _passwordController,
                    ),
                    const SizedBox(height: 60),
                    ElevatedButton(
                      style: CustomTheme.buttonStyle_primaryColor,
                      onPressed: () => ConfirmPassword(),
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
