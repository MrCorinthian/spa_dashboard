import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';
import '../models/dropdown_option.dart';

class CustomDropdown extends StatefulWidget {
  final String text;
  final bool requiredField;
  final bool isExpanded;
  final List<String> options;
  final Function(String?) onChanged;
  final String? selected;

  CustomDropdown({
    Key? key,
    required this.options,
    required this.onChanged,
    this.text = '',
    this.selected,
    this.requiredField = false,
    this.isExpanded = true,
  }) : super(key: key);

  @override
  _CustomDropdownState createState() => _CustomDropdownState();
}

class _CustomDropdownState extends State<CustomDropdown> {
  late List<DropdownMenuItem<String>> _dropdownOptions;

  @override
  void initState() {
    super.initState();
    _dropdownOptions = widget.options
        .map<DropdownMenuItem<String>>((String option) =>
            DropdownMenuItem<String>(value: option, child: Text(option)))
        .toList();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: <Widget>[
        const SizedBox(
          height: 20,
        ),
        Column(
          children: <Widget>[
            Row(
              children: <Widget>[
                Text(
                  widget.text,
                  style: const TextStyle(
                      color: CustomTheme.fillColor, fontSize: 16),
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
          ],
        ),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 7),
          decoration: CustomTheme.dropdownDecoration,
          child: DropdownButton<String>(
            isExpanded: widget.isExpanded,
            underline: const SizedBox(),
            icon: Image.asset(
              'assets/images/down-arrow-filled-circular-button.png',
              fit: BoxFit.contain,
              height: 30,
            ),
            value: widget.selected != '' ? widget.selected : null,
            items: _dropdownOptions,
            onChanged: widget.onChanged,
            style:
                const TextStyle(fontSize: 14, color: CustomTheme.darkGreyColor),
          ),
        )
      ],
    );
  }
}
