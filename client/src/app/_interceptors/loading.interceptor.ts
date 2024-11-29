import { HttpInterceptorFn } from '@angular/common/http';
import { BusyService } from '../_services/busy.service';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busySerrvice = inject(BusyService);
  busySerrvice.busy();

  //wait for 1 second before processing the request
  return next(req).pipe(
    delay(1000),
    //when the request finishes successfully
    finalize(() => {
      busySerrvice.idle();
    })
  );
};
