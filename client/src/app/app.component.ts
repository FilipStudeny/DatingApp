import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './Services/account.service';
import { User } from './Models/User';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
    title: string = 'client';
    users: any;

    constructor(private http: HttpClient, private accountService: AccountService) {}


    
    setCurrentUser(){
        const user: User = JSON.parse(localStorage.getItem('user')!);
        if(!user) return;
        this.accountService.setCurrentUser(user);

    }

    ngOnInit(): void {
        this.setCurrentUser();
    }
}

