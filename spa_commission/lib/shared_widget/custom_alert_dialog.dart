import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';

class CustomAlertDialog extends StatelessWidget implements PreferredSizeWidget {
  final Widget child;

  const CustomAlertDialog({Key? key, required this.child}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      backgroundColor: CustomTheme.darkGreyColor,
      title: Text('Message', style: TextStyle(color: CustomTheme.fillColor)),
      content: Expanded(child: child),
      actions: [
        Center(
          child: TextButton(
            child: const Text(
              'Ok',
              style: TextStyle(color: CustomTheme.fillColor, fontSize: 16),
            ),
            style: TextButton.styleFrom(
              backgroundColor: CustomTheme.primaryColor,
            ),
            onPressed: () {
              Navigator.of(context).pop();
            },
          ),
        )
      ],
    );
  }

  @override
  Size get preferredSize => new Size.fromHeight(80);
}
