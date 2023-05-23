import { MobileComTransaction } from './data/MobileComTransaction';

export class ReportBranch {
  public BranchId!: number;
  public BranchName!: string;
  public TotalBaht!: number;
  public TotalPercentage!: number;
  public Commission!: Array<MobileComTransaction>;
}
