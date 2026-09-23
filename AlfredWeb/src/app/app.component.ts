import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpeechRecognitionService } from 'src/services/speech-recognition.service';

type Message = { role: 'user' | 'assistant'; text: string; time: string };

@Component({ selector: 'app-root', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './app.component.html', styleUrl: './app.component.css' })
export class AppComponent {

  constructor(
    private speechRecognition: SpeechRecognitionService
  ) { }

  text = '';
  isRecording = false;
  messages: Message[] = [
    { role: 'assistant', text: 'Olá! Sou seu assessor financeiro. Você pode registrar uma movimentação por texto ou áudio.', time: 'Agora' },
    { role: 'user', text: 'Paguei R$ 300 ao Carlos referente à compra de material.', time: 'Exemplo' },
    { role: 'assistant', text: 'Entendi: saída de R$ 300,00 para Carlos, referente à compra de material. A integração com a planilha será adicionada na próxima etapa.', time: 'Exemplo' }
  ];

  send(): void {
    const value = this.text.trim();
    if (!value) return;
    this.messages.push({ role: 'user', text: value, time: this.now() });
    this.text = '';
    this.messages.push({ role: 'assistant', text: 'Mensagem recebida. No próximo passo, ela será enviada ao backend para interpretação financeira.', time: this.now() });
  }

  toggleRecording(): void {
    this.isRecording = !this.isRecording;
    if (!this.isRecording) {
      this.messages.push({ role: 'user', text: '🎤 Áudio capturado (mock do frontend).', time: this.now() });
      this.messages.push({ role: 'assistant', text: 'Áudio recebido. A conversão Speech-to-Text será conectada na próxima etapa.', time: this.now() });
    }
  }

  async recordAudio(): Promise<void> {
    if (this.isRecording) {
      this.speechRecognition.stop();
      this.isRecording = false;
      return;
    }

    try {
      this.isRecording = true;

      const text = await this.speechRecognition.start();

      this.isRecording = false;

      this.messages.push({ 
        role: 'user', 
        text: '🎤 Áudio capturado.', 
        time: this.now() });
      
      this.messages.push({
        role: 'assistant',
        text,
        time: this.now()
      });
    } catch (error) {
      this.isRecording = false;
      console.error('Speech recognition error:', error);
    }
  }

  private now(): string { return new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }); }
}
