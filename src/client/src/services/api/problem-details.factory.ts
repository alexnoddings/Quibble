import {HttpErrorResponse} from "@angular/common/http";
import {ProblemDetails} from '$/services/api/problem-details';

const defaultType: string = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
const defaultTitle: string = "Unknown error";
const defaultStatus: number = 500;

function getTypeOrDefault(result: any): string {
  if ("type" in result && typeof result.type === "string") {
    return result.type;
  }
  return defaultType;
}

function getTitleOrDefault(result: any): string {
  if ("title" in result && typeof result.title === "string") {
    return result.title;
  }
  return defaultTitle;
}

function getStatusOrDefault(result: any): number {
  if ("status" in result && typeof result.status === "number") {
    return result.status;
  }
  return defaultStatus;
}

function getDetailOrDefault(result: any): string | undefined {
  if ("detail" in result && typeof result.detail === "string") {
    return result.detail;
  }
  return undefined;
}

function getInstanceOrDefault(result: any): string | undefined {
  if ("instance" in result && typeof result.instance === "string") {
    return result.instance;
  }
  return undefined;
}

export function createProblemDetails(error: any): ProblemDetails {
  if (error instanceof HttpErrorResponse) {
    return createProblemDetails(error.error);
  }

  if (!error || typeof error !== "object") {
    return new ProblemDetails(
      defaultType,
      defaultTitle,
      defaultStatus,
      undefined,
      undefined
    );
  }

  const type = getTypeOrDefault(error);
  const title = getTitleOrDefault(error);
  const status = getStatusOrDefault(error);
  const detail = getDetailOrDefault(error);
  const instance = getInstanceOrDefault(error);
  return new ProblemDetails(type, title, status, detail, instance);
}
