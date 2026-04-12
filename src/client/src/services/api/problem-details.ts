export class ProblemDetails {
  public readonly type: string;
  public readonly title: string;
  public readonly status: number;
  public readonly detail?: string;
  public readonly instance?: string;

  public constructor(
    type: string,
    title: string,
    status: number,
    detail: string | undefined,
    instance: string | undefined
  ) {
    this.type = type;
    this.title = title;
    this.status = status;
    this.detail = detail;
    this.instance = instance;
  }
}
