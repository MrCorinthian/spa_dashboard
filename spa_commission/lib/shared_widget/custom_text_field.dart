import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
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
                    style: TextStyle(color: Colors.red, fontSize: 16))
                : const Text('')
          ],
        ),
        const SizedBox(height: 10),
        TextField(
          controller: widget.controller,
          obscureText: widget.obscureText && hidePassword,
          cursorColor: CustomTheme.backgroundColor,
          keyboardType: widget.keyboardType == "number"
              ? TextInputType.number
              : TextInputType.text,
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
