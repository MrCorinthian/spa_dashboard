import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';
import '../models/dropdown_option.dart';

class CustomDropdownSideBySide extends StatefulWidget {
  final List<DropdownOption> options;
  final Function(String?) onChanged;
  final String? selected;
  final double? maxWidth;

  CustomDropdownSideBySide({
    Key? key,
    required this.options,
    required this.onChanged,
    this.selected,
    this.maxWidth,
  }) : super(key: key);

  @override
  _CustomDropdownSideBySideState createState() =>
      _CustomDropdownSideBySideState();
}

class _CustomDropdownSideBySideState extends State<CustomDropdownSideBySide> {
  late List<DropdownMenuItem<String>> _dropdownOptions;

  @override
  void initState() {
    super.initState();
    _dropdownOptions = widget.options
        .map<DropdownMenuItem<String>>((DropdownOption option) =>
            DropdownMenuItem<String>(
                value: option.label, child: Text(option.label)))
        .toList();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: widget.maxWidth,
      padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 7),
      decoration: CustomTheme.dropdownDecoration,
      child: DropdownButton<String>(
        isExpanded: true,
        underline: const SizedBox(),
        icon: Image.asset(
          'assets/images/down-arrow-filled-circular-button.png',
          fit: BoxFit.contain,
          height: 30,
        ),
        value: widget.selected != '' ? widget.selected : null,
        items: _dropdownOptions,
        onChanged: widget.onChanged,
        style: const TextStyle(fontSize: 14, color: CustomTheme.darkGreyColor),
      ),
    );
  }
}
