#include<winsock2.h>
#include<iostream>
#include<stdlib.h>
#include<stdio.h>
#pragma comment(lib,"Ws2_32.lib")

using namespace std;

void main()
{
	cout << "Server is running ...";

	WORD wVersionRequested;
	WSADATA wsaData;
	int err;
	wVersionRequested = MAKEWORD(2, 2);
	err = WSAStartup(wVersionRequested, &wsaData);

	SOCKET s;
	s = socket(AF_INET, SOCK_DGRAM, 0);

	struct sockaddr_in ad;
	ad.sin_port = htons(1024);
	ad.sin_family = AF_INET;
	ad.sin_addr.s_addr = 0;//подставляет подходящий iр

	bind(s, (struct sockaddr*)&ad, sizeof(ad));

	char b[200], tmр = '\0';
	int l;
	l = sizeof(ad);

	//int recvfrom(SOCKET s, char FAR * buf, int len, int flags, struct sockaddr FAR * from, int FAR * fromlen);

	int rv = recvfrom(s, b, strlen(b), 0, (struct sockaddr*)&ad, &l);

	b[rv] = '\0';
	cout << b << endl;

	if (strlen(b) % 4 == 0) {
		int len = strlen(b) / 2;
		for (int i = 0; i < strlen(b) / 2; i++, len++) {
			char c = b[i];
			b[i] = b[len];
			b[len] = c;
		}
	}

	//int sendto(SOCKET s, const char FAR * buf, int len, int flags, const struct sockaddr FAR * to, int tolen);

	sendto(s, b, strlen(b), 0, (struct sockaddr*)&ad, l);

	closesocket(s);

	WSACleanup();
}
