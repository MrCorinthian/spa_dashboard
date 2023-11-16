import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';
import 'package:package_info_plus/package_info_plus.dart';

class AppVersionWidget extends StatefulWidget {
  const AppVersionWidget({
    Key? key,
  }) : super(key: key);

  @override
  State<AppVersionWidget> createState() => _AppVersionWidgetState();
}

class _AppVersionWidgetState extends State<AppVersionWidget> {
  String _appVersion = "version : -";

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    String version = await getAppVersion();
    setState(() {
      _appVersion = 'version : ' + version;
    });
  }

  Future<String> getAppVersion() async {
    PackageInfo packageInfo = await PackageInfo.fromPlatform();
    return packageInfo.version;
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
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      _appVersion,
                      style: CustomTheme.textStyle_lightText,
                    )
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
