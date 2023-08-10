import { Component, OnInit } from '@angular/core';
import { AccountService } from '../Services/account.service';
import { response } from 'express';
import { Observable, of } from 'rxjs';
import { User } from '../Models/User';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
    model: any = {};

    constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService){ }

    login(){
        this.accountService.login(this.model).subscribe({
            next: () =>  this.router.navigateByUrl("/members")
        });
    }

    loggout(){
        this.accountService.loggout();
        this.router.navigateByUrl('/');
    }

    ngOnInit(): void {
    }

}
