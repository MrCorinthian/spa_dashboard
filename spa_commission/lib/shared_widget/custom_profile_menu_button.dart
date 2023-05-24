import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';

class CustomProfileMenuButton extends StatefulWidget {
  final Function()? onPressed;
  final String text;
  final String imageIconPath;

  const CustomProfileMenuButton({
    Key? key,
    required this.text,
    required this.imageIconPath,
    this.onPressed,
  }) : super(key: key);

  @override
  _CustomProfileMenuButtonState createState() =>
      _CustomProfileMenuButtonState();
}

class _CustomProfileMenuButtonState extends State<CustomProfileMenuButton> {
  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        ElevatedButton(
            style: CustomTheme.buttonStyle_profileMenu,
            onPressed: widget.onPressed,
            child: Row(
              children: <Widget>[
                Image.asset(
                  widget.imageIconPath,
                  fit: BoxFit.contain,
                  height: 30,
                ),
                const SizedBox(
                  width: 10,
                ),
                Text(widget.text,
                    style: CustomTheme.buttonTextStyle_profileMenu),
              ],
            )),
        const SizedBox(height: 10),
      ],
    );
  }
}
