import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-errors',
  imports: [],
  templateUrl: './server-errors.html',
  styleUrl: './server-errors.css',
})
export class ServerErrors {
  protected error : ApiError;
  private router = inject(Router);
  protected showDetails = false;

constructor(){
  const navigation = this.router.currentNavigation();
  this.error = navigation?.extras.state?.['error'];
  console.log(this.error,"error");
}

detailsToggle(){
  this.showDetails = !this.showDetails;
}
}
