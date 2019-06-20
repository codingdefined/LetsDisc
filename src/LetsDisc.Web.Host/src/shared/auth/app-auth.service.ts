import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class AppAuthService {

    constructor(private _tokenAuthService: TokenAuthServiceProxy) { }
    private _isUserAuthenticatedSubject = new BehaviorSubject<boolean>(false);
    isUserAuthenticated: Observable<boolean> = this._isUserAuthenticatedSubject.asObservable();


    logout(reload?: boolean): void {
        abp.auth.clearToken();

        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        };
    }

    /*checkIfUserAuthenticated() {
        return this._tokenAuthService.isUserAuthenticated().pipe(tap(result => {
            this._isUserAuthenticatedSubject.next(result);
        }))
    };*/
}