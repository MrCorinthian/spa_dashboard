import 'package:flutter/material.dart';
import 'package:permission_handler/permission_handler.dart';
import '../../app_theme/app_theme.dart';

class RequestCamera extends StatefulWidget {
  const RequestCamera({super.key});

  @override
  State<RequestCamera> createState() => _RequestCameraState();
}

class _RequestCameraState extends State<RequestCamera> {
  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {}

  @override
  Widget build(BuildContext context) {
    return SafeArea(
        child: Scaffold(
      body: Stack(
        alignment: Alignment.center,
        children: <Widget>[
          buildRequestCamera(context),
          Positioned(
              top: 20,
              left: 20,
              child: GestureDetector(
                onTap: () => Navigator.of(context).pop(),
                child: Container(
                    color: Colors.transparent,
                    child: Image.asset(
                      'assets/images/xmark-primary.png',
                      fit: BoxFit.contain,
                      height: 30,
                    )),
              )),
        ],
      ),
    ));
  }

  Widget buildRequestCamera(BuildContext context) => Scaffold(
          body: Center(
              child: Padding(
        padding: const EdgeInsets.fromLTRB(50, 0, 50, 0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text(
              'To scan QR code, we need access to your device\'s camera',
              textAlign: TextAlign.center,
              style: TextStyle(color: CustomTheme.fillColor, fontSize: 16),
            ),
            const SizedBox(
              height: 20,
            ),
            TextButton(
              child: Text(
                'Open settings',
                style: TextStyle(color: CustomTheme.fillColor, fontSize: 16),
              ),
              style: TextButton.styleFrom(
                backgroundColor: CustomTheme.primaryColor,
              ),
              onPressed: () => openAppSettings(),
            )
          ],
        ),
      )));
}
