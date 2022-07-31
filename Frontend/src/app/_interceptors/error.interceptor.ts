import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                let errorsList = error.error.errors;
                const modalStateErrors = [];

                for (const key in errorsList) {
                  if (errorsList[key]) modalStateErrors.push(errorsList[key])
                }

                throw modalStateErrors.flat();

              } else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:
              this.toastr.error(error?.statusText, error?.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found')
            case 500:
              const navigationExtras: any = { state: { error: error.error } }
              this.router.navigateByUrl('/server-error', navigationExtras)
            default:
              this.toastr.error("Something went wrong");
              console.log(error);
              break;
          }
        }
        return throwError(() => error);
      })
    );
  }
}
