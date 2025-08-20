import { Injectable, signal, effect } from '@angular/core';

export type Theme = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'user-theme';
  private readonly _currentTheme = signal<Theme>('system');
  private readonly _isDark = signal<boolean>(false);

  readonly currentTheme = this._currentTheme.asReadonly();
  readonly isDark = this._isDark.asReadonly();

  constructor() {
    // Effect to apply theme changes to the DOM
    effect(() => {
      this.applyTheme(this._currentTheme());
    });

    // Effect to listen for system theme changes
    this.listenForSystemThemeChanges();
  }

  initializeTheme(): void {
    const savedTheme = this.getSavedTheme();
    this.setTheme(savedTheme);
  }

  setTheme(theme: Theme): void {
    this._currentTheme.set(theme);
    this.saveTheme(theme);
  }

  toggleTheme(): void {
    const currentTheme = this._currentTheme();
    if (currentTheme === 'system') {
      // If system, switch to opposite of current system preference
      const systemIsDark = this.getSystemPreference();
      this.setTheme(systemIsDark ? 'light' : 'dark');
    } else {
      // Toggle between light and dark
      this.setTheme(currentTheme === 'light' ? 'dark' : 'light');
    }
  }

  private applyTheme(theme: Theme): void {
    const root = document.documentElement;
    const isDark = this.calculateIsDark(theme);
    
    this._isDark.set(isDark);
    
    if (isDark) {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }

    // Update meta theme-color for mobile browsers
    this.updateMetaThemeColor(isDark);
  }

  private calculateIsDark(theme: Theme): boolean {
    if (theme === 'system') {
      return this.getSystemPreference();
    }
    return theme === 'dark';
  }

  private getSystemPreference(): boolean {
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
  }

  private listenForSystemThemeChanges(): void {
    window.matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', (e) => {
        if (this._currentTheme() === 'system') {
          this._isDark.set(e.matches);
          this.updateMetaThemeColor(e.matches);
        }
      });
  }

  private updateMetaThemeColor(isDark: boolean): void {
    const metaThemeColor = document.querySelector('meta[name="theme-color"]');
    if (metaThemeColor) {
      metaThemeColor.setAttribute('content', isDark ? '#1f2937' : '#ffffff');
    }
  }

  private saveTheme(theme: Theme): void {
    localStorage.setItem(this.THEME_KEY, theme);
  }

  private getSavedTheme(): Theme {
    const saved = localStorage.getItem(this.THEME_KEY) as Theme;
    return saved || 'system';
  }
}
