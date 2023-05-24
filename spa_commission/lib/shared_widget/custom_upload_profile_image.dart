import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../base_client/base_client.dart';
import '../../app_theme/app_theme.dart';

class CustomUploadProfileImage extends StatefulWidget {
  final Function(File?) onImageSelected;
  final String text;
  final bool obscureText;
  final bool inputDecorationError;
  final bool requiredField;

  const CustomUploadProfileImage({
    Key? key,
    required this.onImageSelected,
    this.text = 'Profile image',
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
            '${BaseClient().getBaseUrl}File/ProfileImage?token=${_token}';
      });
    }
  }

  void _handleImageSelection() async {
    final result = await ImagePicker().pickImage(source: ImageSource.gallery);

    if (result != null) {
      setState(() {
        _image = File(result.path);
      });

      widget.onImageSelected(_image);
    }
  }

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
                  const TextStyle(color: CustomTheme.fillColor, fontSize: 16),
            ),
            const SizedBox(
              width: 5,
            ),
            widget.requiredField
                ? const Text('*',
                    style: TextStyle(color: Colors.red, fontSize: 16))
                : const Text('')
          ],
        ),
        const SizedBox(height: 10),
        GestureDetector(
          onTap: _handleImageSelection,
          child: _image == null
              ? CircleAvatar(
                  radius: 50.0,
                  backgroundColor: CustomTheme.secondaryColor,
                  backgroundImage: _currentImagePath != ''
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
}
