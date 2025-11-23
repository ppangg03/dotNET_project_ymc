import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

@Component ({
    selector: 'app-register',
    standalone:true,
    imports:[ReactiveFormsModule,CommonModule],
    templateUrl: './register.component.html',
    styleUrls:['./register.component.css']
    
})

export class RegisterComponent {
    RegisForm!: FormGroup;
    errorMessage: string = '';
    showPassword: boolean = false;
    returnUrl:string = '/dashboard';

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private route: ActivatedRoute
    )
    {
    
    }

    ngOnInit(): void {
        console.log('Register Component initlalized');

        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dahboard';

        this.RegisForm = this.fb.group({
            username: ['',[Validators.required]],
            Email:['',[Validators.required, Validators.minLength(4)]]
        });
    }

    // onSubmit(): void {
    //     if(this.RegisForm.invalid){
    //         Object.Key
    //     }
    // }
}