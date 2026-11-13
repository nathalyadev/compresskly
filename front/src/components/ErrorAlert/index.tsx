"use client";

import { FiAlertCircle } from "react-icons/fi";

import {
  Alert,
  AlertAction,
  AlertDescription,
  AlertTitle,
  Button,
} from "@/components/ui/components";

interface ErrorAlertProps {
  message: string;
  onDismiss?: () => void;
}

export function ErrorAlert({ message, onDismiss }: ErrorAlertProps) {
  return (
    <Alert variant="destructive">
      <FiAlertCircle />
      <AlertTitle>Não foi possível continuar</AlertTitle>
      <AlertDescription>{message}</AlertDescription>
      {onDismiss ? (
        <AlertAction>
          <Button
            type="button"
            variant="ghost"
            size="sm"
            onClick={onDismiss}
            aria-label="Fechar mensagem de erro"
          >
            Fechar
          </Button>
        </AlertAction>
      ) : null}
    </Alert>
  );
}
