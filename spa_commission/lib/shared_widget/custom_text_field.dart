import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';

class CustomTextField extends StatefulWidget {
  final TextEditingController controller;
  final Function(String)? onChanged;
  final String text;
  final bool obscureText;
  final bool inputDecorationError;
  final bool requiredField;

  const CustomTextField({
    Key? key,
    required this.text,
    required this.controller,
    this.onChanged,
    this.obscureText = false,
    this.inputDecorationError = true,
    this.requiredField = false,
  }) : super(key: key);

  @override
  _CustomTextFieldState createState() => _CustomTextFieldState();
}

class _CustomTextFieldState extends State<CustomTextField> {
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
        TextField(
          controller: widget.controller,
          obscureText: widget.obscureText,
          cursorColor: CustomTheme.backgroundColor,
          decoration: widget.inputDecorationError
              ? CustomTheme.inputDecoration
              : CustomTheme.inputDecorationError,
          onChanged: widget.onChanged,
        )
      ],
    );
  }
}
