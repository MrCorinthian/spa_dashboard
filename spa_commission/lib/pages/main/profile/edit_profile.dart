import 'dart:io';
import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../models/profile_data.dart';
import '../../../models/mobile_user_info.dart';
import '../../../models/responsed_data.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_text_field.dart';
import '../../../shared_widget/custom_dropdown.dart';
import '../../../shared_widget/custom_upload_profile_image.dart';
import '../../../shared_widget/custom_alert_dialog.dart';
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
  final TextEditingController _birthdayController = TextEditingController();
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
  List<String> _province = [];
  List<String> _bank = [];
  List<String> _occupation = [];

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
      final resProvince =
          await BaseClient().get('Data/GetMobileOptionSetting?code=PROVINCE');
      if (resProvince != null) {
        setState(() {
          _province = List<String>.from(json.decode(resProvince));
        });
      }
      final resBank = await BaseClient()
          .get('Data/GetMobileOptionSetting?code=BANK_ACCOUNT');
      if (resBank != null) {
        setState(() {
          _bank = List<String>.from(json.decode(resBank));
        });
      }
      final resOccupation =
          await BaseClient().get('Data/GetMobileOptionSetting?code=OCCUPATION');
      if (resOccupation != null) {
        setState(() {
          _occupation = List<String>.from(json.decode(resOccupation));
        });
      }

      var res =
          await BaseClient().post('User/GetMoblieUserInfo', {'data': _token});
      if (res != null) {
        MobileUserInfo userInfo = MobileUserInfo.fromJson(res);
        setState(() {
          _usernameController.text = userInfo.Username ?? '';
          _firstNameController.text = userInfo.FirstName ?? '';
          _lastNameController.text = userInfo.LastName ?? '';
          _idCardNumberController.text = userInfo.IdCardNumber ?? '';
          if (userInfo.Birthday != null) {
            _birthdayController.text =
                DateFormat('dd MMMM yyyy').format(userInfo.Birthday!);
          }
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
    List<String> messages = [];
    if (_phoneNumberController.text.isNotEmpty) {
      var response = await BaseClient()
          .post('User/Username', {"data": _phoneNumberController.text});
      if (response != null) {
        ResponsedData resPhoneNumber = ResponsedData.fromJson(response);
        if (resPhoneNumber.success == false) {
          if (_profileImage != null) {
            var res = await BaseClient().uploadImage(_profileImage);
            if (res != null) {
              ResponsedData response = ResponsedData.fromJson(res);
              _profilePathController.text = response.data;
            } else {
              validate = false;
              messages.add("Profile image");
            }
          } else {
            validate = false;
            messages.add("Profile image");
          }
          if (_firstNameController.text.isEmpty) {
            validate = false;
            messages.add("First name");
          }
          if (_lastNameController.text.isEmpty) {
            validate = false;
            messages.add("Family name");
          }
          if (_idCardNumberController.text.isEmpty) {
            validate = false;
            messages.add("ID card number");
          }
          if (_provinceController.text.isEmpty) {
            validate = false;
            messages.add("Province");
          }
          if (_occupationController.text.isEmpty) {
            validate = false;
            messages.add("Occupation");
          }
          if (_bankAccountController.text.isEmpty) {
            validate = false;
            messages.add("Bank name");
          }
          if (_bankAccountNumberController.text.isEmpty) {
            validate = false;
            messages.add("Bank account number");
          }
        } else {
          validate = false;
          messages.add("Telephone no. already exists");
        }
      }
    } else {
      validate = false;
      messages.add("Telephone no.");
    }

    if (!validate) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return CustomAlertDialog(
            title: 'Required fields is missing.',
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                for (int i = 0; i < messages.length; i++)
                  Text(' - ' + messages[i],
                      style: TextStyle(color: CustomTheme.fillColor))
              ],
            ),
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
      ProfileData data = ProfileData();
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
                  text: 'First name',
                  requiredField: true,
                  controller: _firstNameController,
                ),
                CustomTextField(
                  text: 'Family name',
                  requiredField: true,
                  controller: _lastNameController,
                ),
                CustomTextField(
                  text: 'ID card number',
                  requiredField: true,
                  controller: _idCardNumberController,
                ),
                CustomTextField(
                  text: 'Birthday',
                  controller: _birthdayController,
                  keyboardType: 'date',
                ),
                CustomTextField(
                  text: 'Nationality',
                  controller: _nationalityController,
                ),
                CustomTextField(
                  text: 'Address',
                  controller: _addressController,
                ),
                _province.length > 0
                    ? CustomDropdown(
                        text: 'Province',
                        requiredField: true,
                        options: _province,
                        selected: _provinceController.text,
                        onChanged: (value) {
                          setState(() {
                            _provinceController.text = value ?? '';
                          });
                        },
                      )
                    : const SizedBox(),
                _occupation.length > 0
                    ? CustomDropdown(
                        text: 'Occupation',
                        requiredField: true,
                        options: _occupation,
                        selected: _occupationController.text,
                        onChanged: (value) {
                          setState(() {
                            _occupationController.text = value ?? '';
                          });
                        },
                      )
                    : const SizedBox(),
                CustomTextField(
                    text: 'Telephone no.',
                    requiredField: true,
                    controller: _phoneNumberController),
                CustomTextField(
                    text: 'Email address', controller: _emailController),
                CustomTextField(text: 'Line ID', controller: _lineController),
                CustomTextField(
                    text: 'Whatsapp ID', controller: _whatsappController),
                CustomTextField(
                    text: 'Company name', controller: _companyNameController),
                CustomTextField(
                    text: 'Company tax ID', controller: _companyTexController),
                _bank.length > 0
                    ? CustomDropdown(
                        text: 'Bank name',
                        requiredField: true,
                        options: _bank,
                        selected: _bankAccountController.text,
                        onChanged: (value) {
                          setState(() {
                            _bankAccountController.text = value ?? '';
                          });
                        },
                      )
                    : const SizedBox(),
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
                // const SizedBox(height: 30),
                // ElevatedButton(
                //   style: CustomTheme.buttonStyle_secondaryColor,
                //   onPressed: () => {Navigator.pop(context)},
                //   child: const Text('Back to Profile',
                //       style: CustomTheme.buttonTextStyle_fillColor),
                // ),
                const SizedBox(height: 20),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
