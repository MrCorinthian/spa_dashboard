import 'dart:convert';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';
import '../../providers/AuthProvider/auth_provider.dart';
import '../../base_client/base_client.dart';
import '../../models/register_data.dart';
import '../../models/responsed_data.dart';
import '../../models/dropdown.dart';
import '../../app_theme/app_theme.dart';
import '../../shared_function/shared_function.dart';
import '../../shared_widget/custom_page_route_builder.dart';
import '../../shared_widget/custom_app_bar.dart';
import '../../shared_widget/custom_text_field.dart';
import '../../shared_widget/custom_dropdown.dart';
import '../../shared_widget/custom_upload_profile_image.dart';
import '../../shared_widget/custom_alert_dialog.dart';
import '../../shared_widget/custom_loading.dart';
import '../../shared_widget/custom_upload_id_card_image.dart';
import '../../shared_widget/privacy_policy.dart';

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
  bool _isChecked = false;
  List<Dropdown> _province = [];
  List<Dropdown> _bank = [];
  List<Dropdown> _occupation = [];
  List<Dropdown> _companyTypeOfUsage = [];
  final Uri _urlPrivacy =
      Uri.parse('https://manage.urban-partners-group.com/Privacy');

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
        if (_companyTypeOfUsage.length > 0)
          _companyTypeOfUsageController.text =
              _companyTypeOfUsage[0].Id.toString();
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

  Future<void> _openLink() async {
    if (!await launchUrl(_urlPrivacy)) {
      throw Exception('Could not launch $_urlPrivacy');
    }
  }

  void _validateData() async {
    var validate = true;
    List<String> messages = [];
    if (_phoneNumberController.text.isNotEmpty &&
        _phoneNumberController.text.length == 10) {
      var response = await BaseClient()
          .post('User/Username', {"data": _phoneNumberController.text});
      if (response != null) {
        ResponsedData resPhoneNumber = ResponsedData.fromJson(response);
        if (resPhoneNumber.success == false) {
          if (_profileImage == null) {
            validate = false;
            messages.add("Profile proto / ภาพถ่ายใบหน้า");
          }
          if (_IdCardImage != null) {
            var res = await BaseClient().uploadAttachment(_IdCardImage);
            if (res != null) {
              ResponsedData response = ResponsedData.fromJson(res);
              _idCardPathController.text = response.data;
            } else {
              validate = false;
              messages.add("ID card photo / ภาพถ่ายบัตรประชาชน ");
            }
          } else {
            validate = false;
            messages.add("ID card photo / ภาพถ่ายบัตรประชาชน ");
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
          if (_passwordController.text.isEmpty ||
              _confirmPasswordController.text.isEmpty ||
              _passwordController.text != _confirmPasswordController.text ||
              _passwordController.text.length < 6) {
            validate = false;
            messages.add("Create your password / สร้างรหัสผ่าน");
          }
          if (_emailController.text.isNotEmpty &&
              !Validator.isValidEmail(_emailController.text)) {
            validate = false;
            messages.add("Create your password / สร้างรหัสผ่าน");
          }
          if (!_isChecked) {
            validate = false;
            messages.add("Privacy policy");
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
      _register();
    }
  }

  _register() async {
    setState(() {
      _loading = true;
    });

    if (_profileImage != null) {
      var res = await BaseClient().uploadImage(_profileImage);
      if (res != null) {
        ResponsedData response = ResponsedData.fromJson(res);
        _profilePathController.text = response.data;
      }
    }

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
    data.CompanyTypeOfUsage = _companyTypeOfUsageController.text;
    data.CompanyName = _companyNameController.text;
    data.CompanyTaxId = _companyTaxController.text;
    data.CompanyAddress = _companyAddressController.text;
    data.Bank = _bankController.text;
    data.BankAccountNumber = _bankAccountNumberController.text;
    data.ProfilePath = _profilePathController.text;
    data.IdCardPath = _idCardPathController.text;

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
        barrierDismissible: false,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return CustomLoading(isLoading: _loading, child: buildRegister());
  }

  Widget buildRegister() => GestureDetector(
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
                        'Register',
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
                      text: 'Telephone no. / เบอร์โทรศัพท์',
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
                          text:
                              'Company tax ID / เลขประจำตัวผู้เสียภาษีบริษัท ',
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
                      keyboardType: 'number'),
                  CustomTextField(
                      text: 'Create your password / สร้างรหัสผ่าน ',
                      placeholder: 'Must have at least 6 characters',
                      requiredField: true,
                      obscureText: true,
                      controller: _passwordController),
                  CustomTextField(
                    text: 'Confirm your password / ยืนยันรหัสผ่าน',
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
                  Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Theme(
                          data: Theme.of(context).copyWith(
                              unselectedWidgetColor: CustomTheme.fillColor),
                          child: CheckboxListTile(
                            activeColor: CustomTheme.primaryColor,
                            checkColor: CustomTheme.fillColor,
                            value: _isChecked,
                            onChanged: (value) {
                              setState(() {
                                _isChecked = value!;
                              });
                            },
                            title: GestureDetector(
                              onTap: () => Navigator.push(
                                  context,
                                  CustomPageRouteBuilder.bottomToTop(
                                      PrivacyPolicy())),
                              child: RichText(
                                text: TextSpan(
                                  children: [
                                    TextSpan(
                                      text: 'I have read and accept the ',
                                      style: TextStyle(
                                          color: CustomTheme.fillColor,
                                          fontSize:
                                              18), // Set white color for the first part
                                    ),
                                    TextSpan(
                                      text: 'Privacy Policy',
                                      style: TextStyle(
                                          color: CustomTheme.primaryColor,
                                          fontSize:
                                              18), // Use primary color (red) for the second part
                                    ),
                                  ],
                                ),
                              ),
                            ),
                            controlAffinity: ListTileControlAffinity.leading,
                          )),
                    ],
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
      ));
}
