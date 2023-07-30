import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../Services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{

    @Output() cancelRegistration = new EventEmitter();
    model: any = {};

    constructor(private accountService: AccountService, private toastr: ToastrService){}
    
    register(){
        this.accountService.register(this.model).subscribe({
            next: () => {
                this.cancel();
            },
            error: error => this.toastr.error(error.error)
        })
    }

    cancel(){
        this.cancelRegistration.emit(false); //cancel registration
    }

    ngOnInit(): void {
    }

}
