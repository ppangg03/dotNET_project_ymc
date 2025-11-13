import { HttpClient } from "@angular/common/http";
import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { Route, Router } from "@angular/router";

@Component({
    selector:'login',
    templateUrl:'./login.companent.html'
})
export class LoginComponent {
    invalidLogin!: boolean;

    constructor(private router: Router,private http: HttpClient){}

    login(form: NgForm) {
        const credentials = {
            'username' : form.value.usename,
            'password' : form.value.password
        }

        this.http.post("",credentials)
        .subscribe(response => {
            const token = (<any>response).token;
            localStorage.setItem("jwt",token);
            this.invalidLogin = false;
            this.router.navigate(["/"]);
        },err => {
            this.invalidLogin = true;
        })
    }
}