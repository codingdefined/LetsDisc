import { Component, OnInit, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'app-confirm-email',
    template: ''
})
export class ConfirmEmailComponent extends AppComponentBase implements OnInit {

    id: number;
    code: string;

    constructor(injector: Injector,
        private route: ActivatedRoute,
        private _tokenAuthService: TokenAuthServiceProxy,
        private router: Router)
    {
        super(injector);
    }

    ngOnInit() {
        this.route.queryParamMap.subscribe(params => {
            this.id = +params.get('userId');
            this.code = params.get('code');
            this.confirmEmail(this.id, this.code)
        })
    }

    private confirmEmail(id: number, code: string) {
        this._tokenAuthService.confirmEmail(id, code)
            .subscribe((result: number) => {
                if (result > 0) {
                    this.notify.info(this.l('Email Confirmed, please login'));    
                    this.router.navigate(['account/login']);
                } else {
                    this.notify.info(this.l('Email Not Confirmed, please register'));
                    this.router.navigate(['account/register']);
                }
                
            });
    }
}
