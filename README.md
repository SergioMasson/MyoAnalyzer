# Bem vindo ao projeto MyoAnalyzer!

O projeto MyoAnalyzer tem como objetivo promover o estudo na área de processamento de sinais através da criação de uma plataforma simples de aquisição e processamento de dados, que possibilite a utilização do sensor Myo por pessoas que não sejam programadoras. 

## Detalhes da implementação 

Nossa plataforma de desenvolvimento durante todo o projeto vem sendo o Visual Studio da Microsoft, e a linguagem de programação para todo o código é o C#. Com isso temos acesso a muitas ferramentas para criação de interfaces gráficas através dos frameworks WPF e Windows Forms. Para compilar o projeto atual são necessárias as seguintes ferramentas: 

- Visual Studio 2015 (Qualquer versão. A edição Community é totalmente gratuita e da acesso a 95% de todas as fetures do Visual Studio. Pode ser baixada gratuitamente através do Link: https://www.visualstudio.com/pt-br/downloads/)

- Visual studio 2015 click once Publishing tools. Essa ferramenta permite a criação rápida e descomplicada de executáveis utilizando Visual Studio sem que seja necessário passar por grandes processos de assinatura de código. Para instala-la certifique-se de que ela está selecionada na aba de features durante a instalação de Visual Studio 2015. (Ela não vem selecionada por Default)   

![Visual Studio click once Publishing tools](https://i.stack.imgur.com/fEZEX.png)

- Deixe as demais opções de instalação do visual Studio selecionadas como vieram por default.

- Também é necessário baixar e instalar o Myo SDK, e Myo Connect (disponível em https://developer.thalmic.com/downloads)

### Acesso ao MYO SDK

A maior parte do SDK de desensolvimento promovido pela Thalmic é escrito em C/C++, a grande desvantagem desse método é que essas linguagens são mais complexas e portanto possuem um fluxo de desenvolvimento e uma produtividade mais lenta do que liguagens como Java, Python e C#. No entanto é possível chamar código em C++ de dentro dessas plataformas e é essa a tecnologia que utilizamos para ter acesso ao SDK nativo do Myo
