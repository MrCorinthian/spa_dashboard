import 'dart:convert';
import 'dart:ffi';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/AuthProvider/auth_provider.dart';
import '../../base_client/base_client.dart';
import '../../models/register_data.dart';
import '../../models/responsed_data.dart';
import '../../app_theme/app_theme.dart';
import '../../shared_widget/custom_app_bar.dart';
import '../../shared_widget/custom_text_field.dart';
import '../../shared_widget/custom_dropdown.dart';
import '../../shared_widget/custom_upload_profile_image.dart';
import '../../shared_widget/custom_alert_dialog.dart';
import '../../shared_widget/custom_loading.dart';

class RegisterPage extends StatefulWidget {
  @override
  _RegisterPageState createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final TextEditingController _firstNameController = TextEditingController();
  final TextEditingController _lastNameController = TextEditingController();
  final TextEditingController _idCardNumberController = TextEditingController();
  final TextEditingController _birthdayController = TextEditingController();
  final TextEditingController _nationalityController = TextEditingController();
  final TextEditingController _addressController = TextEditingController();
  final TextEditingController _provinceController = TextEditingController();
  final TextEditingController _occupationController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  bool _isPasswordMatched = true;
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

  bool _loading = false;
  String profileImagePath = '';
  File? _profileImage;
  List<String> _province = [];
  List<String> _bank = [];
  List<String> _occupation = [];

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    setState(() {
      _loading = true;
    });

    final resProvince =
        await BaseClient().get('Data/GetMobileOptionSetting?code=PROVINCE');
    if (resProvince != null) {
      setState(() {
        _province = List<String>.from(json.decode(resProvince));
      });
    }
    final resBank =
        await BaseClient().get('Data/GetMobileOptionSetting?code=BANK_ACCOUNT');
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

    setState(() {
      _loading = false;
    });
  }

  @override
  void dispose() {
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  void _handleUpload(File? file) {
    if (file != null) {
      setState(() {
        _profileImage = file;
      });
    }
  }

  void _validateData() async {
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
          if (_passwordController.text.isEmpty ||
              _confirmPasswordController.text.isEmpty ||
              _passwordController.text != _confirmPasswordController.text) {
            validate = false;
            messages.add("Password");
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
      _register();
    }
  }

  _register() async {
    setState(() {
      _loading = true;
    });

    RegisterData data = RegisterData();
    data.Password = _passwordController.text;
    data.FirstName = _firstNameController.text;
    data.LastName = _lastNameController.text;
    data.IdCardNumber = _idCardNumberController.text;
    data.Birthday = _birthdayController.text;
    data.Nationality = _nationalityController.text;
    data.Address = _addressController.text;
    data.Province = _provinceController.text;
    data.Occupation = _occupationController.text;
    data.PhoneNumber = _phoneNumberController.text;
    data.Email = _emailController.text;
    data.LineId = _lineController.text;
    data.WhatsAppId = _whatsappController.text;
    data.CompanyName = _companyNameController.text;
    data.CompanyTexId = _companyTexController.text;
    data.BankAccount = _bankAccountController.text;
    data.BankAccountNumber = _bankAccountNumberController.text;
    data.ProfilePath = _profilePathController.text;

    final json = data.toJson();
    var res = await BaseClient().post('Register/Register', json);

    setState(() {
      _loading = false;
    });

    if (res != null) {
      Navigator.pop(context);
      Provider.of<AuthProvider>(context, listen: false).login(res);
    } else {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return CustomAlertDialog(
            title: 'Message',
            child: Text('Please check the information.',
                style: TextStyle(color: CustomTheme.fillColor)),
          );
        },
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return CustomLoading(isLoading: _loading, child: buildRegister());
  }

  Widget buildRegister() => Scaffold(
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
                        'Register',
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
                    keyboardType: 'number',
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
                    controller: _phoneNumberController,
                    keyboardType: 'number',
                  ),
                  CustomTextField(
                      text: 'Email address', controller: _emailController),
                  CustomTextField(text: 'Line ID', controller: _lineController),
                  CustomTextField(
                      text: 'Whatsapp ID', controller: _whatsappController),
                  CustomTextField(
                      text: 'Company name', controller: _companyNameController),
                  CustomTextField(
                    text: 'Company tax ID',
                    controller: _companyTexController,
                    keyboardType: 'number',
                  ),
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
                      controller: _bankAccountNumberController,
                      keyboardType: 'number'),
                  CustomTextField(
                      text: 'Password',
                      requiredField: true,
                      obscureText: true,
                      controller: _passwordController),
                  CustomTextField(
                    text: 'Confirm password',
                    requiredField: true,
                    obscureText: true,
                    controller: _confirmPasswordController,
                    inputDecorationError: _isPasswordMatched,
                    onChanged: (value) {
                      setState(() {
                        _isPasswordMatched = _passwordController.text == value;
                      });
                    },
                  ),
                  const SizedBox(height: 60),
                  ElevatedButton(
                    style: CustomTheme.buttonStyle_primaryColor,
                    onPressed: _validateData,
                    child: const Text('Confirm',
                        style: CustomTheme.buttonTextStyle_fillColor),
                  ),
                  // const SizedBox(height: 30),
                  // ElevatedButton(
                  //   style: CustomTheme.buttonStyle_secondaryColor,
                  //   onPressed: () => {Navigator.pop(context)},
                  //   child: const Text('Back to Sign in',
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
