import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:permission_handler/permission_handler.dart';
import '../../base_client/base_client.dart';
import '../../app_theme/app_theme.dart';
import 'request_camera.dart';
import '../shared_widget/custom_page_route_builder.dart';

class CustomUploadIdCardImage extends StatefulWidget {
  final Function(File?) onImageSelected;
  final String text;
  final bool obscureText;
  final bool inputDecorationError;
  final bool requiredField;

  const CustomUploadIdCardImage({
    Key? key,
    required this.onImageSelected,
    this.text = 'ID card image',
    this.obscureText = false,
    this.inputDecorationError = true,
    this.requiredField = false,
  }) : super(key: key);

  @override
  _CustomUploadIdCardImageState createState() =>
      _CustomUploadIdCardImageState();
}

class _CustomUploadIdCardImageState extends State<CustomUploadIdCardImage> {
  String _token = '';
  String _currentImagePath = '';
  File? _image;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      setState(() {
        _currentImagePath =
            '${BaseClient().getBaseUrl}File/AttachmentImage?token=${_token}';
      });
    }
  }

  void _handleImageSelection(String type) async {
    bool allowCamera = await requestCameraPermission();
    if (allowCamera) {
      final result = await ImagePicker().pickImage(
          source: type == 'camera' ? ImageSource.camera : ImageSource.gallery);

      if (result != null) {
        setState(() {
          _image = File(result.path);
        });

        widget.onImageSelected(_image);
      }
    }
  }

  Widget buildChooseImageSource() => Scaffold(
        backgroundColor: Color.fromRGBO(0, 0, 0, 0.2),
        body: Center(
          child: Padding(
            padding: EdgeInsets.all(10),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                ElevatedButton(
                  style: CustomTheme.buttonStyle_primaryColor,
                  onPressed: () {
                    Navigator.of(context).pop();
                    _handleImageSelection('camera');
                  },
                  child: const Text('Camera',
                      style: CustomTheme.buttonTextStyle_fillColor),
                ),
                const SizedBox(
                  height: 24,
                ),
                ElevatedButton(
                  style: CustomTheme.buttonStyle_primaryColor,
                  onPressed: () {
                    Navigator.of(context).pop();
                    _handleImageSelection('');
                  },
                  child: const Text('Gallery',
                      style: CustomTheme.buttonTextStyle_fillColor),
                )
              ],
            ),
          ),
        ),
      );

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        const SizedBox(
          height: 20,
        ),
        Row(
          children: <Widget>[
            Text(
              widget.text,
              style:
                  const TextStyle(color: CustomTheme.fillColor, fontSize: 18),
            ),
            const SizedBox(
              width: 5,
            ),
            widget.requiredField
                ? const Text('*',
                    style: TextStyle(color: Colors.red, fontSize: 18))
                : const Text('')
          ],
        ),
        const SizedBox(height: 10),
        GestureDetector(
            // onTap: () => showDialog(
            //   context: context,
            //   builder: (BuildContext context) {
            //     return buildChooseImageSource();
            //   },
            // ),
            onTap: () => _handleImageSelection('camera'),
            child: _image == null
                ? Container(
                    width: 200,
                    height: 120,
                    decoration: BoxDecoration(
                      color: CustomTheme.secondaryColor,
                      image: _currentImagePath.isNotEmpty
                          ? DecorationImage(
                              image: NetworkImage(_currentImagePath),
                              fit: BoxFit.cover,
                            )
                          : null,
                    ),
                    child: const Icon(
                      Icons.camera_alt,
                      size: 30.0,
                      color: CustomTheme.fillColor,
                    ))
                : Container(
                    width: 200,
                    height: 120,
                    decoration: BoxDecoration(
                      color: CustomTheme.secondaryColor,
                      image: DecorationImage(
                        image: FileImage(_image!),
                        fit: BoxFit.cover,
                      ),
                    ),
                    child: const Icon(
                      Icons.camera_alt,
                      size: 30.0,
                      color: CustomTheme.fillColor,
                    ))),
      ],
    );
  }

  Future requestCameraPermission() async {
    bool allowCamera = false;
    var status = await Permission.camera.status;
    if (status.isDenied) {
      var request = await Permission.camera.request();
      if (request.isGranted || request.isLimited) {
        allowCamera = true;
      } else {
        Navigator.push(
          context,
          CustomPageRouteBuilder.bottomToTop(RequestCamera()),
        );
      }
    } else {
      allowCamera = true;
    }
    return allowCamera;
  }
}
