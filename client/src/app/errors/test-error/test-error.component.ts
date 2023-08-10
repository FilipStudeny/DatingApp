import { HttpClient } from '@angular/common/http';
import { NotExpr } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit{

    baseURL = 'https://localhost:7202/api/';
    validationErrors: string[] = [];

    constructor(private http: HttpClient) {}

    get404Error(){
        this.http.get(this.baseURL + 'buggy/not-found').subscribe({
            next: response => console.log(response),
            error: error => console.log(error)
        });
    }

    get400Error(){
        this.http.get(this.baseURL + 'buggy/bad-request').subscribe({
            next: response => console.log(response),
            error: error => console.log(error)
        });
    }

    get500Error(){
        this.http.get(this.baseURL + 'buggy/server-error').subscribe({
            next: response => console.log(response),
            error: error => console.log(error)
        });
    }

    get401Error(){
        this.http.get(this.baseURL + 'buggy/auth').subscribe({
            next: response => console.log(response),
            error: error => console.log(error)
        });
    }

    get400ValidationError(){
        this.http.post(this.baseURL + 'account/register', {}).subscribe({
            next: response => console.log(response),
            error: error => {
                console.log(error),
                this.validationErrors = error
            }
        });
    }

    ngOnInit(): void {
    }

}
