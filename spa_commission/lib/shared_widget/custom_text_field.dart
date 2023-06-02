import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import '../../app_theme/app_theme.dart';

class CustomTextField extends StatefulWidget {
  final TextEditingController controller;
  final Function(String)? onChanged;
  final String text;
  final bool obscureText;
  final bool inputDecorationError;
  final bool requiredField;
  final String keyboardType;

  const CustomTextField({
    Key? key,
    required this.text,
    required this.controller,
    this.onChanged,
    this.obscureText = false,
    this.inputDecorationError = true,
    this.requiredField = false,
    this.keyboardType = 'normal',
  }) : super(key: key);

  @override
  _CustomTextFieldState createState() => _CustomTextFieldState();
}

class _CustomTextFieldState extends State<CustomTextField> {
  bool hidePassword = true;

  void _selectDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(1930),
      lastDate: DateTime(DateTime.now().year),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: ColorScheme.dark(
              primary: CustomTheme.darkGreyColor,
              onPrimary: CustomTheme.primaryColor,
              onSurface: CustomTheme.primaryColor,
            ),
            textButtonTheme: TextButtonThemeData(
              style: TextButton.styleFrom(
                foregroundColor:
                    Color.fromARGB(255, 0, 0, 0), // button text color
              ),
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null) {
      setState(() {
        widget.controller.text = DateFormat('dd MMMM yyyy').format(picked);
      });
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
        TextField(
          controller: widget.controller,
          obscureText: widget.obscureText && hidePassword,
          cursorColor: CustomTheme.backgroundColor,
          onTap: () {
            widget.keyboardType == "date" ? _selectDate(context) : null;
          },
          keyboardType: widget.keyboardType == "number"
              ? TextInputType.number
              : (widget.keyboardType == "date"
                  ? TextInputType.none
                  : TextInputType.text),
          inputFormatters: widget.keyboardType == "number"
              ? [FilteringTextInputFormatter.digitsOnly]
              : [],
          decoration: widget.inputDecorationError
              ? InputDecoration(
                  filled: true,
                  fillColor: CustomTheme.fillColor,
                  border: OutlineInputBorder(
                    borderSide: BorderSide.none,
                  ),
                  suffixIcon: widget.obscureText
                      ? IconButton(
                          icon: Icon(
                            !hidePassword
                                ? Icons.visibility
                                : Icons.visibility_off,
                          ),
                          color: Color.fromARGB(255, 192, 192, 192),
                          onPressed: () {
                            setState(() {
                              hidePassword = !hidePassword;
                            });
                          },
                        )
                      : null)
              : InputDecoration(
                  filled: true,
                  fillColor: CustomTheme.fillColor,
                  enabledBorder: OutlineInputBorder(
                    borderSide: BorderSide(
                      color: Colors.red,
                    ),
                  ),
                  focusedBorder: OutlineInputBorder(
                      borderSide: BorderSide(
                    color: Colors.red,
                  )),
                  border: OutlineInputBorder(
                    borderSide: BorderSide.none,
                  ),
                  suffixIcon: widget.obscureText
                      ? IconButton(
                          icon: Icon(
                            !hidePassword
                                ? Icons.visibility
                                : Icons.visibility_off,
                          ),
                          color: Color.fromARGB(255, 192, 192, 192),
                          onPressed: () {
                            setState(() {
                              hidePassword = !hidePassword;
                            });
                          },
                        )
                      : null),
          onChanged: widget.onChanged,
        )
      ],
    );
  }
}
