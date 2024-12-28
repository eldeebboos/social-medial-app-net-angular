import { HttpInterceptorFn } from '@angular/common/http';
import { BusyService } from '../_services/busy.service';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { environment } from '../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busySerrvice = inject(BusyService);
  busySerrvice.busy();

  //wait for 1 second before processing the request
  return next(req).pipe(
    environment.production ? identity : delay(1000),
    //when the request finishes successfully
    finalize(() => {
      busySerrvice.idle();
    })
  );
};
