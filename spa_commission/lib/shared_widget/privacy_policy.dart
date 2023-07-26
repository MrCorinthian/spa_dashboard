import 'package:flutter/material.dart';
import '../../../base_client/base_client.dart';
import '../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';

class PrivacyPolicy extends StatefulWidget {
  const PrivacyPolicy({
    Key? key,
  }) : super(key: key);

  @override
  State<PrivacyPolicy> createState() => _PrivacyPolicyState();
}

class _PrivacyPolicyState extends State<PrivacyPolicy> {
  String _privacyPolicy = "";

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    var res =
        await BaseClient().get('Data/GetMobileOption?code=PRIVACY_POLICY');
    if (res != null) {
      String rpString = res;
      rpString = rpString
          .replaceAll('<br />', '')
          .replaceAll('\\n', '\n')
          .replaceAll('\\r', '')
          .replaceAll('\\"', '\"');
      setState(() {
        _privacyPolicy = rpString;
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
              children: [
                Row(
                  children: const <Widget>[
                    Text(
                      'Privacy policy',
                      style: CustomTheme.headerPageName,
                    )
                  ],
                ),
                const SizedBox(
                  height: 20,
                ),
                Text(
                  _privacyPolicy,
                  style: TextStyle(color: CustomTheme.fillColor),
                )
              ],
            ),
          ),
        ),
      ),
    );
  }
}
