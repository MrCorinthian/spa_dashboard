import 'dart:io';
import 'package:flutter/material.dart';
import 'package:qr_code_scanner/qr_code_scanner.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:geolocator/geolocator.dart';
import 'package:permission_handler/permission_handler.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_alert_dialog.dart';

class QrScanPage extends StatefulWidget {
  const QrScanPage({super.key});

  @override
  State<StatefulWidget> createState() => _QrScanPageState();
}

class _QrScanPageState extends State<QrScanPage> {
  String _token = '';
  bool _processing = false;
  bool _allow_camera = true;
  final GlobalKey qrKey = GlobalKey(debugLabel: 'QR');

  Barcode? barcode;
  QRViewController? controller;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    requestCameraPermission();
  }

  @override
  void dispose() {
    controller?.dispose();
    super.dispose();
  }

  @override
  void reassemble() async {
    super.reassemble();
    if (Platform.isAndroid) {
      await controller!.pauseCamera();
    }
    controller!.resumeCamera();
  }

  @override
  Widget build(BuildContext context) {
    return SafeArea(
        child: Scaffold(
      body: Stack(
        alignment: Alignment.center,
        children: <Widget>[
          _allow_camera ? buildQrView(context) : buildRequsetCamera(context),
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
          if (_allow_camera) Positioned(bottom: 30, child: buildResult()),
        ],
      ),
    ));
  }

  Widget buildPopup() {
    return this._processing
        ? const Center(
            child: CircularProgressIndicator(
            color: CustomTheme.primaryColor,
          ))
        : AlertDialog(
            backgroundColor: CustomTheme.darkGreyColor,
            content: Column(mainAxisSize: MainAxisSize.min, children: const [
              const SizedBox(
                height: 20,
              ),
              Image(
                image: AssetImage('assets/images/circle-check.png'),
                width: 100,
              ),
              const SizedBox(
                height: 20,
              ),
              Text(
                'Completed',
                style: TextStyle(color: CustomTheme.fillColor),
              ),
            ]),
            actions: [
              this._processing
                  ? const Text('')
                  : Center(
                      child: TextButton(
                        child: Text(
                          'Ok',
                          style: TextStyle(
                              color: CustomTheme.fillColor, fontSize: 16),
                        ),
                        style: TextButton.styleFrom(
                          backgroundColor: CustomTheme.primaryColor,
                        ),
                        onPressed: () {
                          this.barcode = null;
                          Navigator.of(context).pop();
                          Navigator.of(context).pop(true);
                        },
                      ),
                    ),
            ],
          );
  }

  Widget buildResult() => Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
            color: Colors.white, borderRadius: BorderRadius.circular(8)),
        child: Column(children: [
          Text('Scan a QR Code.'),
        ]),
      );

  Widget buildRequsetCamera(BuildContext context) => Scaffold(
          body: Center(
              child: Padding(
        padding: EdgeInsets.fromLTRB(50, 0, 50, 0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
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

  Widget buildQrView(BuildContext context) => QRView(
        key: qrKey,
        onQRViewCreated: onQRViewCreated,
        overlay: QrScannerOverlayShape(
          borderColor: CustomTheme.primaryColor,
          borderRadius: 10,
          borderLength: 20,
          borderWidth: 10,
        ),
      );

  void onQRViewCreated(QRViewController controller) async {
    setState(() {
      this.controller = controller;
    });
    controller.scannedDataStream.listen((barcode) async {
      if (this.barcode == null) {
        this.barcode = barcode;

        this._processing = true;

        this.controller!.pauseCamera();
        showDialog(
          context: context,
          builder: (BuildContext context) {
            return buildPopup();
          },
        );

        await Future.delayed(Duration(seconds: 1));

        double? latitude = null;
        double? longitude = null;

        try {
          Position position = await Geolocator.getCurrentPosition(
            desiredAccuracy: LocationAccuracy.high,
          );
          latitude = position.latitude;
          longitude = position.longitude;
        } catch (ex) {}

        var res = await BaseClient().post('Commission/CommissionReceipt', {
          'receiptCode': this.barcode!.code,
          'token': this._token,
          'latitude': "${latitude}",
          'longitude': "${longitude}"
        });
        if (res != null) {
          setState(() {
            this._processing = false;
            Navigator.of(context).pop();
            showDialog(
              context: context,
              builder: (BuildContext context) {
                return buildPopup();
              },
            );
          });
        } else {
          setState(() {
            this._processing = false;
            Navigator.of(context).pop();
            showDialog(
              context: context,
              builder: (BuildContext context) {
                return AlertDialog(
                  backgroundColor: CustomTheme.darkGreyColor,
                  title: const Text('Message',
                      style: TextStyle(color: CustomTheme.fillColor)),
                  content: const Text(
                    'QR Code is expired or invalid',
                    style: TextStyle(color: CustomTheme.fillColor),
                  ),
                  actions: [
                    Center(
                      child: TextButton(
                        child: Text(
                          'Ok',
                          style: TextStyle(
                              color: CustomTheme.fillColor, fontSize: 16),
                        ),
                        style: TextButton.styleFrom(
                          backgroundColor: CustomTheme.primaryColor,
                        ),
                        onPressed: () {
                          Navigator.of(context).pop();
                          this.controller!.resumeCamera();
                        },
                      ),
                    )
                  ],
                );
              },
            );
          });
        }
      }
    });
  }

  void requestCameraPermission() async {
    var status = await Permission.camera.status;
    if (status.isDenied) {
      if (await Permission.camera.request().isGranted) {
        setState(() {
          _allow_camera = true;
        });
      } else {
        setState(() {
          _allow_camera = false;
        });
      }
    } else {
      setState(() {
        _allow_camera = true;
      });
    }
  }
}
