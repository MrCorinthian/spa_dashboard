import 'dart:io';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../models/register_data.dart';
import '../../../models/mobile_user_info.dart';
import '../../../models/responsed_data.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_text_field.dart';
import '../../../shared_widget/custom_dropdown.dart';
import '../../../shared_widget/custom_upload_profile_image.dart';
import '../../../constant_value/constant_value.dart' as constent;

class EditProfilePage extends StatefulWidget {
  @override
  _EditProfilePageState createState() => _EditProfilePageState();
}

class _EditProfilePageState extends State<EditProfilePage> {
  String _token = '';
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _firstNameController = TextEditingController();
  final TextEditingController _lastNameController = TextEditingController();
  final TextEditingController _idCardNumberController = TextEditingController();
  final TextEditingController _nationalityController = TextEditingController();
  final TextEditingController _addressController = TextEditingController();
  final TextEditingController _provinceController = TextEditingController();
  final TextEditingController _occupationController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _phoneNumberController = TextEditingController();
  final TextEditingController _lineController = TextEditingController();
  final TextEditingController _whatsappController = TextEditingController();
  final TextEditingController _companyNameController = TextEditingController();
  final TextEditingController _companyTexController = TextEditingController();
  final TextEditingController _bankAccountController = TextEditingController();
  final TextEditingController _bankAccountNumberController =
      TextEditingController();
  final TextEditingController _profilePathController = TextEditingController();

  String profileImagePath = '';
  File? _profileImage;

  @override
  void dispose() {
    super.dispose();
  }

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      var res =
          await BaseClient().post('User/GetMoblieUserInfo', {'data': _token});
      if (res != null) {
        MobileUserInfo userInfo = MobileUserInfo.fromJson(res);
        setState(() {
          _usernameController.text = userInfo.Username ?? '';
          _firstNameController.text = userInfo.FirstName ?? '';
          _lastNameController.text = userInfo.LastName ?? '';
          _idCardNumberController.text = userInfo.IdCardNumber ?? '';
          _nationalityController.text = userInfo.Nationality ?? '';
          _addressController.text = userInfo.Address ?? '';
          _provinceController.text = userInfo.Province ?? '';
          _occupationController.text = userInfo.Occupation ?? '';
          _phoneNumberController.text = userInfo.PhoneNumber ?? '';
          _emailController.text = userInfo.Email ?? '';
          _lineController.text = userInfo.LineId ?? '';
          _whatsappController.text = userInfo.WhatsAppId ?? '';
          _companyNameController.text = userInfo.CompanyName ?? '';
          _companyTexController.text = userInfo.CompanyTexId ?? '';
          _bankAccountController.text = userInfo.BankAccount ?? '';
          _bankAccountNumberController.text = userInfo.BankAccountNumber ?? '';
        });
      }
    }
  }

  void _handleUpload(File? file) {
    if (file != null) {
      setState(() {
        _profileImage = file;
      });
    }
  }

  validateData() async {
    var validate = true;

    if (_profileImage != null) {
      var res = await BaseClient().uploadImage(_profileImage);
      if (res != null) {
        ResponsedData response = ResponsedData.fromJson(res);
        _profilePathController.text = response.data;
      }
    }

    if (_usernameController.text.isEmpty) {
      validate = false;
    } else if (_firstNameController.text.isEmpty) {
      validate = false;
    } else if (_lastNameController.text.isEmpty) {
      validate = false;
    } else if (_idCardNumberController.text.isEmpty) {
      validate = false;
    } else if (_provinceController.text.isEmpty) {
      validate = false;
    } else if (_phoneNumberController.text.isEmpty) {
      validate = false;
    } else if (_bankAccountController.text.isEmpty) {
      validate = false;
    } else if (_bankAccountNumberController.text.isEmpty) {
      validate = false;
    }

    if (!validate) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text('Message'),
            content: Text('Please check the information.'),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: Text('OK'),
              ),
            ],
          );
        },
      );
    } else {
      updateProfile();
    }
  }

  updateProfile() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      RegisterData data = RegisterData();
      data.Token = _token;
      data.Password = '';
      data.TitleName = '';
      data.Username = _usernameController.text;
      data.FirstName = _firstNameController.text;
      data.LastName = _lastNameController.text;
      data.IdCardNumber = _idCardNumberController.text;
      data.Nationality = _nationalityController.text;
      data.Address = _addressController.text;
      data.Province = _provinceController.text;
      data.Occupation = _occupationController.text;
      data.PhoneNunber = _phoneNumberController.text;
      data.Email = _emailController.text;
      data.LineId = _lineController.text;
      data.WhatsAppId = _whatsappController.text;
      data.CompanyName = _companyNameController.text;
      data.CompanyTexId = _companyTexController.text;
      data.BankAccount = _bankAccountController.text;
      data.BankAccountNumber = _bankAccountNumberController.text;
      data.ProfilePath = _profilePathController.text;

      final json = data.toJson();
      var res = await BaseClient().post('User/UpdateUserInfo', json);
      if (res != null) {
        Navigator.pop(context);
      }
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
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                Row(
                  children: const <Widget>[
                    Text(
                      'Edit personal information',
                      style: CustomTheme.headerPageName,
                    )
                  ],
                ),
                CustomUploadProfileImage(
                  requiredField: true,
                  onImageSelected: _handleUpload,
                ),
                CustomTextField(
                  text: 'Username',
                  requiredField: true,
                  controller: _usernameController,
                ),
                CustomTextField(
                  text: 'First Name',
                  requiredField: true,
                  controller: _firstNameController,
                ),
                CustomTextField(
                  text: 'Family Name',
                  requiredField: true,
                  controller: _lastNameController,
                ),
                CustomTextField(
                  text: 'ID Card number',
                  requiredField: true,
                  controller: _idCardNumberController,
                ),
                CustomTextField(
                  text: 'Nationality',
                  controller: _nationalityController,
                ),
                CustomTextField(
                  text: 'Address',
                  controller: _addressController,
                ),
                CustomDropdown(
                  text: 'Province',
                  requiredField: true,
                  options: constent.PROVINCE,
                  selected: _provinceController.text,
                  onChanged: (value) {
                    setState(() {
                      _provinceController.text = value ?? '';
                    });
                  },
                ),
                CustomDropdown(
                  text: 'Occupation',
                  requiredField: true,
                  options: constent.OCCUPATION,
                  selected: _occupationController.text,
                  onChanged: (value) {
                    setState(() {
                      _occupationController.text = value ?? '';
                    });
                  },
                ),
                CustomTextField(
                    text: 'Telephone',
                    requiredField: true,
                    controller: _phoneNumberController),
                // CustomTextField(
                //     text: 'Username', controller: _usernameController),
                CustomTextField(
                    text: 'Email Address', controller: _emailController),
                CustomTextField(text: 'Line ID', controller: _lineController),
                CustomTextField(
                    text: 'Whatsapp ID', controller: _whatsappController),
                CustomTextField(
                    text: 'Company Name', controller: _companyNameController),
                CustomTextField(
                    text: 'Company tex ID', controller: _companyTexController),
                CustomDropdown(
                  text: 'Bank account',
                  requiredField: true,
                  options: constent.BANK_ACCOUNT,
                  selected: _bankAccountController.text,
                  onChanged: (value) {
                    setState(() {
                      _bankAccountController.text = value ?? '';
                    });
                  },
                ),
                CustomTextField(
                    text: 'Bank account number',
                    requiredField: true,
                    controller: _bankAccountNumberController),
                const SizedBox(height: 60),
                ElevatedButton(
                  style: CustomTheme.buttonStyle_primaryColor,
                  onPressed: validateData,
                  child: const Text('Update',
                      style: CustomTheme.buttonTextStyle_fillColor),
                ),
                const SizedBox(height: 30),
                ElevatedButton(
                  style: CustomTheme.buttonStyle_secondaryColor,
                  onPressed: () => {Navigator.pop(context)},
                  child: const Text('Back to Profile',
                      style: CustomTheme.buttonTextStyle_fillColor),
                ),
                const SizedBox(height: 20),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
