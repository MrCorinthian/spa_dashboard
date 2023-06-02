import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../providers/AuthProvider/auth_provider.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_profile_menu_button.dart';
import '../../../shared_widget/custom_profile_widget.dart';
import '../../../shared_widget/custom_page_route_builder.dart';
import './edit_profile.dart';
import './commission_tier_info.dart';
import './change_password.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({
    Key? key,
  }) : super(key: key);

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  String _token = '';

  @override
  void initState() {
    super.initState();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {}
  }

  Future<void> logout() => SharedPreferences.getInstance().then((prefs) =>
      BaseClient().post('User/Logout', {'data': _token}).then((res) => prefs
          .remove('spa_login_token')
          .then((value) =>
              Provider.of<AuthProvider>(context, listen: false).logout())));

  Widget buildLoadingWidget() => const Center(
        child: CircularProgressIndicator(
          color: CustomTheme.primaryColor,
        ),
      );

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              const SizedBox(height: 20),
              CustomProfileMenuButton(
                text: 'Edit personal information',
                imageIconPath: 'assets/images/person-edit.png',
                onPressed: () => Navigator.push(context,
                    CustomPageRouteBuilder.leftToRight(EditProfilePage())),
              ),
              CustomProfileMenuButton(
                text: 'Change password',
                imageIconPath: 'assets/images/change-password.png',
                onPressed: () => Navigator.push(context,
                    CustomPageRouteBuilder.leftToRight(ChangePasswordPage())),
              ),
              CustomProfileMenuButton(
                text: 'Commission tier',
                imageIconPath: 'assets/images/com-tier.png',
                onPressed: () => Navigator.push(
                    context,
                    CustomPageRouteBuilder.leftToRight(
                        CommisionTierInfoPage())),
              ),
              CustomProfileMenuButton(
                text: 'Sign out',
                imageIconPath: 'assets/images/sign-out.png',
                onPressed: logout,
              ),
            ],
          ),
        )
      ],
    );
  }
}
