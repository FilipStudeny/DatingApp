import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../Services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{

    @Output() cancelRegistration = new EventEmitter();
    registerForm: FormGroup = new FormGroup({});
    maxDate: Date = new Date();
    validationErrors: string[] | undefined;
    
    constructor(private accountService: AccountService, 
                private toastr: ToastrService, 
                private fb: FormBuilder, private router: Router) { }
    
    register(){
        const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
        const values = {...this.registerForm.value, dateOfBirth: dob};
        this.accountService.register(values).subscribe({
            next: () => {
                this.router.navigateByUrl('/members');
            },
            error: errors => {
                this.validationErrors = errors;
            }
        })
    }

    cancel(){
        this.cancelRegistration.emit(false); //cancel registration
    }

    private getDateOnly(dob: string | undefined){
        if(!dob) return;
        let dobDate = new Date(dob);
        return new Date(dobDate.setMinutes(dobDate.getMinutes()-dobDate.getTimezoneOffset())).toISOString().slice(0,10);
    }

    initializeForm() {
        this.registerForm = this.fb.group({
            gender: ['male'],
            username: ['', Validators.required],
            knownAs: ['', Validators.required],
            dateOfBirth: ['', Validators.required],
            city: ['', Validators.required],
            country: ['', Validators.required],

            password: ['', [Validators.minLength(4), Validators.required, Validators.maxLength(10)]],
            confirmPassword: ['', [this.matchValues('password'), Validators.required]],
        })

        this.registerForm.controls['password'].valueChanges.subscribe({
            next: () => {
                this.registerForm.controls['confirmPassword'].updateValueAndValidity()
            }
        })
    }

    matchValues(matchTo: string): ValidatorFn{
        return (control: AbstractControl) => {
            return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true};
        }
    }

    ngOnInit(): void {
        this.initializeForm();
        this.maxDate = new Date();
        this.maxDate?.setFullYear(this.maxDate.getFullYear() -18);
    }

}
