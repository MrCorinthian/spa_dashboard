import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:uuid/uuid.dart';
import '../../base_client/base_client.dart';
import '../../app_theme/app_theme.dart';
import 'request_camera.dart';
import '../shared_widget/custom_page_route_builder.dart';

class CustomUploadProfileImage extends StatefulWidget {
  final Function(File?) onImageSelected;
  final String text;
  final bool obscureText;
  final bool inputDecorationError;
  final bool requiredField;

  const CustomUploadProfileImage({
    Key? key,
    required this.onImageSelected,
    this.text = 'Profile photo / ภาพถ่ายใบหน้า',
    this.obscureText = false,
    this.inputDecorationError = true,
    this.requiredField = false,
  }) : super(key: key);

  @override
  _CustomUploadProfileImageState createState() =>
      _CustomUploadProfileImageState();
}

class _CustomUploadProfileImageState extends State<CustomUploadProfileImage> {
  String _token = '';
  String _currentImagePath = '';
  File? _image;

  String _status = "";
  String _request = "";

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    var status = await Permission.camera.status;
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      final res = await BaseClient().get('File/ProfileImage?token=${_token}');
      if (res != null) {
        setState(() {
          _currentImagePath =
              '${BaseClient().getBaseUrl}File/ProfileImage?token=${_token}&c=${generateUniqueId()}';
        });
      }
    }
  }

  void _handleImageSelection(String type) async {
    final rawImage = await ImagePicker().pickImage(
        source: type == 'camera' ? ImageSource.camera : ImageSource.gallery,
        preferredCameraDevice: CameraDevice.front);

    if (rawImage != null) {
      final compressedImage = await compressImage(rawImage);
      if (compressedImage != null) {
        setState(() {
          _image = File(compressedImage.path);
        });

        widget.onImageSelected(_image);
      }
    }

    bool allowCamera = await requestCameraPermission();
    // if (allowCamera) {

    // }
  }

  compressImage(XFile imageFile) async {
    int quality = 90;
    final filePath = imageFile.path;
    final lastIndex = filePath.lastIndexOf(new RegExp(r'.jp'));
    final compressedPath = "${filePath.substring(0, (lastIndex))}_compress.jpg";
    final originalFile = File(filePath);

    final result = await FlutterImageCompress.compressAndGetFile(
      originalFile.absolute.path,
      compressedPath,
      quality: quality,
      minWidth: 800,
    );

    return result!;
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
              ? CircleAvatar(
                  radius: 50.0,
                  backgroundColor: CustomTheme.secondaryColor,
                  backgroundImage: _currentImagePath.isNotEmpty
                      ? NetworkImage(_currentImagePath)
                      : null,
                  child: const Icon(
                    Icons.camera_alt,
                    size: 30.0,
                    color: CustomTheme.fillColor,
                  ),
                )
              : CircleAvatar(
                  radius: 50.0,
                  backgroundColor: CustomTheme.secondaryColor,
                  backgroundImage: _image != null ? FileImage(_image!) : null,
                  child: const Icon(
                    Icons.camera_alt,
                    size: 30.0,
                    color: CustomTheme.fillColor,
                  ),
                ),
        ),
      ],
    );
  }

  Future requestCameraPermission() async {
    bool allowCamera = false;
    var status = await Permission.camera.status;
    if (status.isDenied)
      _status = "isDenied";
    else if (status.isGranted)
      _status = "isGranted";
    else if (status.isLimited)
      _status = "isLimited";
    else if (status.isPermanentlyDenied)
      _status = "isPermanentlyDenied";
    else if (status.isProvisional)
      _status = "isProvisional";
    else if (status.isRestricted) _status = "isRestricted";
    if (status.isDenied || status.isPermanentlyDenied) {
      var request = await Permission.camera.request();
      if (request.isDenied)
        _request = "isDenied";
      else if (request.isGranted)
        _request = "isGranted";
      else if (request.isLimited)
        _request = "isLimited";
      else if (request.isPermanentlyDenied)
        _request = "isPermanentlyDenied";
      else if (request.isProvisional)
        _request = "isProvisional";
      else if (request.isRestricted) _request = "isRestricted";
      if (request.isDenied || request.isPermanentlyDenied) {
        Navigator.push(
          context,
          CustomPageRouteBuilder.bottomToTop(RequestCamera()),
        );
      } else {
        allowCamera = true;
      }
    } else {
      allowCamera = true;
    }
    return allowCamera;
  }

  String generateUniqueId() {
    var uuid = Uuid();
    return uuid.v4();
  }
}
