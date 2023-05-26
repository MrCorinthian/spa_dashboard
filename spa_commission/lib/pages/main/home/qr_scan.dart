import 'dart:io';
import 'package:flutter/material.dart';
import 'package:qr_code_scanner/qr_code_scanner.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../app_theme/app_theme.dart';

class QrScanPage extends StatefulWidget {
  const QrScanPage({super.key});

  @override
  State<StatefulWidget> createState() => _QrScanPageState();
}

class _QrScanPageState extends State<QrScanPage> {
  String _token = '';
  bool _processing = false;
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
          buildQrView(context),
          Positioned(bottom: 30, child: buildResult())
        ],
      ),
    ));
  }

  // Widget buildLoadingScreen() => const Scaffold(
  //       body: Center(
  //         child: CircularProgressIndicator(
  //           color: CustomTheme.primaryColor,
  //         ),
  //       ),
  //     );

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
                        child: const Text(
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
                          Navigator.of(context).pop();
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

        var res = await BaseClient().post('Commission/CommissionReceipt',
            {'receiptCode': this.barcode!.code, 'token': this._token});
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
          Navigator.of(context).pop();
        }
      }
    });
  }
}
