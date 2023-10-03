import 'dart:io';
import 'dart:convert';
import 'package:intl/intl.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import '../../../models/profile_data.dart';
import '../../../models/mobile_user_info.dart';
import '../../../models/responsed_data.dart';
import '../../../models/dropdown.dart';
import '../../../app_theme/app_theme.dart';
import '../../../shared_function/shared_function.dart';
import '../../../shared_widget/custom_app_bar.dart';
import '../../../shared_widget/custom_text_field.dart';
import '../../../shared_widget/custom_dropdown.dart';
import '../../../shared_widget/custom_upload_profile_image.dart';
import '../../../shared_widget/custom_alert_dialog.dart';
import '../../../shared_widget/custom_loading.dart';
import '../../../shared_widget/custom_upload_id_card_image.dart';
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
  final TextEditingController _companyTypeOfUsageController =
      TextEditingController();
  final TextEditingController _companyNameController = TextEditingController();
  final TextEditingController _companyTaxController = TextEditingController();
  final TextEditingController _companyAddressController =
      TextEditingController();
  final TextEditingController _bankController = TextEditingController();
  final TextEditingController _bankAccountNumberController =
      TextEditingController();
  final TextEditingController _profilePathController = TextEditingController();
  final TextEditingController _idCardPathController = TextEditingController();

  bool _loading = false;
  String profileImagePath = '';
  File? _profileImage;
  String _IdCardImagePath = '';
  File? _IdCardImage;
  List<Dropdown> _province = [];
  List<Dropdown> _bank = [];
  List<Dropdown> _occupation = [];
  List<Dropdown> _companyTypeOfUsage = [];

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
    setState(() {
      _loading = true;
    });

    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      final resProvince =
          await BaseClient().get('Data/GetMobileOptionSetting?code=PROVINCE');
      if (resProvince != null) {
        final jsonData = json.decode(resProvince) as List;
        setState(() {
          _province = jsonData.map((m) => Dropdown.fromJson(m)).toList();
        });
      }
      final resBank =
          await BaseClient().get('Data/GetMobileOptionSetting?code=BANK_NAME');
      if (resBank != null) {
        final jsonData = json.decode(resBank) as List;
        setState(() {
          _bank = jsonData.map((m) => Dropdown.fromJson(m)).toList();
        });
      }
      final resOccupation =
          await BaseClient().get('Data/GetMobileOptionSetting?code=OCCUPATION');
      if (resOccupation != null) {
        final jsonData = json.decode(resOccupation) as List;
        setState(() {
          _occupation = jsonData.map((m) => Dropdown.fromJson(m)).toList();
        });
      }
      final resCompanyTypeOfUsage = await BaseClient()
          .get('Data/GetMobileOptionSetting?code=COM_TYPE_OF_USAGE');
      if (resCompanyTypeOfUsage != null) {
        final jsonData = json.decode(resCompanyTypeOfUsage) as List;
        setState(() {
          _companyTypeOfUsage =
              jsonData.map((m) => Dropdown.fromJson(m)).toList();
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
          _companyTypeOfUsageController.text =
              userInfo.CompanyTypeOfUsage ?? '';
          _companyNameController.text = userInfo.CompanyName ?? '';
          _companyTaxController.text = userInfo.CompanyTaxId ?? '';
          _companyAddressController.text = userInfo.CompanyAddress ?? '';
          _bankController.text = userInfo.Bank ?? '';
          _bankAccountNumberController.text = userInfo.BankAccountNumber ?? '';

          _loading = false;
        });
      } else {
        setState(() {
          _loading = false;
        });
      }
    }
  }

  void _handleUploadProfile(File? file) {
    if (file != null) {
      setState(() {
        _profileImage = file;
      });
    }
  }

  void _handleUploadIdCard(File? file) {
    if (file != null) {
      setState(() {
        _IdCardImage = file;
      });
    }
  }

  validateData() async {
    setState(() {
      _loading = true;
    });
    var validate = true;
    List<String> messages = [];
    if (_phoneNumberController.text.isNotEmpty &&
        _phoneNumberController.text.length == 10) {
      var response = await BaseClient().post('User/Telephone',
          {"Token": _token, "PhoneNumber": _phoneNumberController.text});
      if (response != null) {
        ResponsedData resPhoneNumber = ResponsedData.fromJson(response);
        if (resPhoneNumber.success == true) {
          if (_IdCardImage != null) {
            var res =
                await BaseClient().uploadUserAttachment(_token, _IdCardImage);
            if (res != null) {
              ResponsedData response = ResponsedData.fromJson(res);
              _idCardPathController.text = response.data;
            } else {
              // validate = false;
              // messages.add("ID card image");
            }
          } else {
            // validate = false;
            // messages.add("ID card image");
          }
          if (_firstNameController.text.isEmpty) {
            validate = false;
            messages.add("First name / ชื่อจริง");
          }
          if (_lastNameController.text.isEmpty) {
            validate = false;
            messages.add("Family name / นามสกุล");
          }
          if (_idCardNumberController.text.isEmpty) {
            validate = false;
            messages.add("ID card no. / เลขบัตรประชาชน");
          }
          if (_provinceController.text.isEmpty) {
            validate = false;
            messages.add("Province / จังหวัด");
          }
          if (_occupationController.text.isEmpty) {
            validate = false;
            messages.add("Occupation / อาชีพ");
          }
          if (_companyTypeOfUsageController.text == "99") {
            if (_companyNameController.text.isEmpty) {
              validate = false;
              messages.add("Company name / ชื่อบริษัท");
            }
            if (_companyTaxController.text.isEmpty) {
              validate = false;
              messages.add("Company tax ID / เลขประจำตัวผู้เสียภาษีบริษัท");
            }
            if (_companyAddressController.text.isEmpty) {
              validate = false;
              messages.add("Company address / ที่อยู่บริษัท");
            }
          }
          if (_bankController.text.isEmpty) {
            validate = false;
            messages.add("Bank name / บัญชีธนาคาร");
          }
          if (_bankAccountNumberController.text.isEmpty) {
            validate = false;
            messages.add("Bank account no. / เลขที่บัญชี");
          }
          if (_emailController.text.isNotEmpty &&
              !Validator.isValidEmail(_emailController.text)) {
            validate = false;
            messages.add("Email address / อีเมล");
          }
        } else {
          validate = false;
          messages.add("Telephone no. already exists");
        }
      }
    } else {
      validate = false;
      messages.add("Telephone no. / เบอร์โทรศัพท์");
    }

    if (!validate) {
      setState(() {
        _loading = false;
      });
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
        barrierDismissible: false,
      );
    } else {
      updateProfile();
    }
  }

  updateProfile() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('spa_login_token') ?? '';
    if (_token != '') {
      if (_profileImage != null) {
        var res = await BaseClient().uploadImage(_profileImage);
        if (res != null) {
          ResponsedData response = ResponsedData.fromJson(res);
          _profilePathController.text = response.data;
        }
      }

      ProfileData data = ProfileData();
      data.Token = _token;
      data.Password = '';
      data.TitleName = '';
      data.Username = _usernameController.text;
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
      data.CompanyTypeOfUsage = _companyTypeOfUsageController.text;
      data.CompanyName = _companyNameController.text;
      data.CompanyTaxId = _companyTaxController.text;
      data.CompanyAddress = _companyAddressController.text;
      data.Bank = _bankController.text;
      data.BankAccountNumber = _bankAccountNumberController.text;
      data.ProfilePath = _profilePathController.text;
      data.IdCardPath = _idCardNumberController.text;

      final json = data.toJson();
      var res = await BaseClient().post('User/UpdateUserInfo', json);

      setState(() {
        _loading = false;
      });

      if (res != null) {
        Navigator.of(context).pop(true);
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
          barrierDismissible: false,
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return CustomLoading(isLoading: _loading, child: buildEditProfile());
  }

  Widget buildEditProfile() => GestureDetector(
      onTap: () {
        FocusManager.instance.primaryFocus?.unfocus();
      },
      child: Scaffold(
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
                    onImageSelected: _handleUploadProfile,
                  ),
                  CustomTextField(
                    text: 'First name / ชื่อจริง',
                    requiredField: true,
                    controller: _firstNameController,
                  ),
                  CustomTextField(
                    text: 'Family name / นามสกุล',
                    requiredField: true,
                    controller: _lastNameController,
                  ),
                  CustomTextField(
                      text: 'ID card no. / เลขบัตรประชาชน',
                      requiredField: true,
                      controller: _idCardNumberController,
                      keyboardType: 'number',
                      maxLength: 13),
                  CustomUploadIdCardImage(
                    requiredField: true,
                    onImageSelected: _handleUploadIdCard,
                  ),
                  CustomTextField(
                    text: 'Birthday / วันเกิด',
                    controller: _birthdayController,
                    keyboardType: 'date',
                  ),
                  CustomTextField(
                    text: 'Nationality / สัญชาติ',
                    controller: _nationalityController,
                  ),
                  CustomTextField(
                    text: 'Address / ที่อยู่',
                    controller: _addressController,
                  ),
                  _province.length > 0
                      ? CustomDropdown(
                          text: 'Province / จังหวัด',
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
                          text: 'Occupation / อาชีพ',
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
                      text: 'Telephone no. / เบอร์โทรศัพท์ ',
                      requiredField: true,
                      controller: _phoneNumberController,
                      keyboardType: 'number',
                      maxLength: 10),
                  CustomTextField(
                      text: 'Email address / อีเมล',
                      controller: _emailController),
                  CustomTextField(text: 'Line ID', controller: _lineController),
                  CustomTextField(
                      text: 'Whatsapp ID', controller: _whatsappController),
                  _companyTypeOfUsage.length > 0
                      ? CustomDropdown(
                          text: 'Type of usage / ประเภทผู้ใช้',
                          requiredField: true,
                          options: _companyTypeOfUsage,
                          selected: _companyTypeOfUsageController.text,
                          onChanged: (value) {
                            setState(() {
                              _companyTypeOfUsageController.text = value ?? '';
                            });
                          },
                        )
                      : const SizedBox(),
                  _companyTypeOfUsageController.text == "99"
                      ? CustomTextField(
                          text: 'Company name / ชื่อบริษัท',
                          requiredField:
                              _companyTypeOfUsageController.text == "99",
                          controller: _companyNameController)
                      : const SizedBox(),
                  _companyTypeOfUsageController.text == "99"
                      ? CustomTextField(
                          text: 'Company tax ID / เลขประจำตัวผู้เสียภาษีบริษัท',
                          requiredField:
                              _companyTypeOfUsageController.text == "99",
                          controller: _companyTaxController,
                          keyboardType: 'number',
                        )
                      : const SizedBox(),
                  _companyTypeOfUsageController.text == "99"
                      ? CustomTextField(
                          text: 'Company address / ที่อยู่บริษัท',
                          requiredField:
                              _companyTypeOfUsageController.text == "99",
                          controller: _companyAddressController,
                        )
                      : const SizedBox(),
                  _bank.length > 0
                      ? CustomDropdown(
                          text: 'Bank name / บัญชีธนาคาร',
                          requiredField: true,
                          options: _bank,
                          selected: _bankController.text,
                          onChanged: (value) {
                            setState(() {
                              _bankController.text = value ?? '';
                            });
                          },
                        )
                      : const SizedBox(),
                  CustomTextField(
                    text: 'Bank account no. / เลขที่บัญชี',
                    requiredField: true,
                    controller: _bankAccountNumberController,
                    keyboardType: 'number',
                  ),
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
      ));
}
