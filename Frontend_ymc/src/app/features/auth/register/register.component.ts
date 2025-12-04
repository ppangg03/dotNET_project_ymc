import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from "@angular/forms";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { Subscription } from "rxjs/internal/Subscription";
import { AuthService } from "../../../core/services/auth.service";
import { interval } from "rxjs/internal/observable/interval";
import { AuthResponse, ErrorResponse } from "../../../core/models/auth.model";

@Component ({
    selector: 'app-register',
    standalone:true,
    imports:[ReactiveFormsModule,
      CommonModule, 
      FormsModule, 
      RouterModule],
    templateUrl: './register.component.html',
    styleUrls:['./register.component.css']
    
})

export class RegisterComponent implements OnInit, OnDestroy {
  registerForm: FormGroup;
  otpForm: FormGroup;
  currentStep = 1;
  errorMessage = '';
  otpErrorMessage = '';
  otpSuccessMessage = '';
  isLoading = false;
  isVerifying = false;
  showPassword = false;
  showConfirmPassword = false;
  passwordStrength = 0;

  otpDigits: string[] = ['', '', '', '', '', ''];
  resendCountdown = 0;
  private countdownSubscription?: Subscription;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private route: ActivatedRoute
    )
    {
    this.registerForm = this.fb.group({
      username: ['', [
        Validators.required,
        // Validators.minLength(3),
        // Validators.pattern(/^[a-zA-Z0-9_]+$/)
      ]],
      email: ['', [
        Validators.required,
        Validators.email
      ]],
      password: ['', [
        Validators.required,
        // Validators.minLength(5),
        // Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });

     this.otpForm = this.fb.group({
      otp: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.registerForm.get('password')?.valueChanges.subscribe(password => {
      this.passwordStrength = this.calculatePasswordStrength(password);
    });
  }

     ngOnInit(): void {
      // Debug form
        this.registerForm.statusChanges.subscribe(status => {
        console.log('Form Status:', status);
        console.log('Form Valid:', this.registerForm.valid);
        console.log('Form Errors:', this.registerForm.errors);
        console.log('Username Valid:', this.registerForm.get('username')?.valid);
        console.log('Email Valid:', this.registerForm.get('email')?.valid);
        console.log('Password Valid:', this.registerForm.get('password')?.valid);
        console.log('Confirm Password Valid:', this.registerForm.get('confirmPassword')?.valid);
        });
     }

  ngOnDestroy(): void {
    if (this.countdownSubscription) {
      this.countdownSubscription.unsubscribe();
    }
  }

   passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    return password && confirmPassword && password.value === confirmPassword.value 
      ? null : { passwordMismatch: true };
  }

  calculatePasswordStrength(password: string): number {
    let strength = 0;
    if (!password) return strength;
    if (password.length >= 8) strength += 25;
    if (password.length >= 12) strength += 25;
    if (/[a-z]/.test(password)) strength += 12.5;
    if (/[A-Z]/.test(password)) strength += 12.5;
    if (/\d/.test(password)) strength += 12.5;
    if (/[@$!%*?&]/.test(password)) strength += 12.5;
    return Math.min(strength, 100);
  }

  getPasswordStrengthText(): string {
    if (this.passwordStrength < 40) return 'Weak';
    if (this.passwordStrength < 70) return 'Medium';
    return 'Strong';
  }


  //step1 send OTP
  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const userData = {
        username: this.registerForm.value.username,
        email: this.registerForm.value.email,
        password: this.registerForm.value.password
      };

      this.authService.sendOTP(userData.email).subscribe({
        next: (response: any) => {
          this.isLoading = false;
          this.currentStep = 2;
          this.startResendCountdown();
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to send OTP.';
        }
      });
    }
  }

  //step 2 Verify OTP & Register
  onVerifyOTP(): void {
    if (!this.isOtpComplete()) return;

    this.isVerifying = true;
    this.otpErrorMessage = '';
    this.otpSuccessMessage = '';

    const otp = this.otpDigits.join('');
    const userData = {
      username: this.registerForm.value.username,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      otp: otp
    };

    this.authService.register(userData).subscribe({
      next: (response: any) => {
        this.isVerifying = false;
        this.otpSuccessMessage = 'Registration successful! Redirecting...';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (error) => {
        this.isVerifying = false;
        this.otpErrorMessage = error.error?.message || 'Invalid OTP.';
        this.otpDigits = ['', '', '', '', '', ''];
        (document.getElementById('otp-0') as HTMLInputElement)?.focus();
      }
    });
  }


  //OTP Input Handlers
  onOtpInput(event: any, index: number): void {
    const value = event.target.value;
    if (value.length === 1 && index < 5) {
      document.getElementById(`otp-${index + 1}`)?.focus();
    }
  }

  onOtpKeydown(event: KeyboardEvent, index: number): void {
    if (event.key === 'Backspace' && !this.otpDigits[index] && index > 0) {
      document.getElementById(`otp-${index - 1}`)?.focus();
    }
  }

  onOtpPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const pastedData = event.clipboardData?.getData('text');
    
    if (pastedData && /^\d{6}$/.test(pastedData)) {
      this.otpDigits = pastedData.split('');
      const lastInput = document.getElementById('otp-5') as HTMLInputElement;
      lastInput?.focus();
    }
  }

  isOtpComplete(): boolean {
    return this.otpDigits.every(digit => digit.length === 1);
  }


  //ReSent OTP
  onResendOTP(): void {
    if (this.resendCountdown > 0) return;
    const email = this.registerForm.value.email;

    this.authService.sendOTP(email).subscribe({
      next: (response: AuthResponse) => {
        this.isLoading = false;
        this.currentStep = 2;
        this.otpErrorMessage = '';
         this.otpSuccessMessage = 'New OTP sent!';
        this.startResendCountdown();
        setTimeout(() => this.otpSuccessMessage = '', 3000);
      },
      error: (error: ErrorResponse) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Failed to send OTP.';
      }
    });
  }

   startResendCountdown(): void {
    this.resendCountdown = 60;
    if (this.countdownSubscription) 
      this.countdownSubscription.unsubscribe();
    this.countdownSubscription = interval(1000).subscribe(() => {
       this.resendCountdown--; 
       if (this.resendCountdown <= 0) 
        this.countdownSubscription?.unsubscribe(); 
      });
  }

  goBackToForm(): void {
    this.currentStep = 1;
    this.otpDigits = ['', '', '', '', '', ''];
    this.otpErrorMessage = '';
    this.otpSuccessMessage = '';
  }
}