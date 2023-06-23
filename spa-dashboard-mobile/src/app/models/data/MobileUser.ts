export class MobileUser {
  public Id!: number;
  public Username!: string;
  public Password!: string;
  public TitleName!: string;
  public FirstName!: string;
  public LastName!: string;
  public IdCardNumber!: string;
  public Nationality!: string;
  public Birthday!: Date;
  public Address!: string;
  public Province!: string;
  public Occupation!: string;
  public PhoneNumber!: string;
  public Email!: string;
  public LineId!: string;
  public WhatsAppId!: string;
  public CompanyTypeOfUsage!: string;
  public CompanyName!: string;
  public CompanyTaxId!: string;
  public BankAccount!: string;
  public BankAccountNumber!: string;
  public ProfilePath!: string;
  public Active!: string;
  public Created!: Date;
  public CreatedBy!: string;
  public Updated!: Date;
  public UpdatedBy!: string;

  //extension
  public ConfirmPassword!: string;
  public BirthdayString!: string;
  public IdCardPath!: string;
  idCardFilename!: string;
}
