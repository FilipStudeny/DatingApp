import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{


    registerMode: boolean = false;
    users: any;

    constructor(private http: HttpClient){}

    togleRegisterForm(): void{
        this.registerMode = !this.registerMode;
    }


    getUsers(){
        this.http.get("https://localhost:7202/api/user").subscribe({
            next: response => this.users = response,
            error: error => console.log(error),
            complete: () => console.log('Request has completed')
        });
    }

    cancelRegisterMode(event: boolean){
        this.registerMode = event;
    }

    ngOnInit(): void {
        this.getUsers();
    }

}
